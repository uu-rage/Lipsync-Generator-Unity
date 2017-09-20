using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LipSyncGenerator;
using System.Linq;
using System;
using LipSyncGenerator.Audio;
using LipSyncGenerator.Emotions;
using LipSyncGenerator.Helpers;

[Serializable]
public class LipSyncManager : MonoBehaviour {
    public SkinnedMeshRenderer characterMesh; // character on which lip sync will be performed
    public float blendSpeed;

    public LipSyncGenerator.LipSync lipSync = new LipSyncGenerator.LipSync()
    {
        pathManager = new UnityPathManager()
    };

    public enum ReadMode
    {
        Inspector,
        XML
    }

    [Header("Read Mode")]
    public ReadMode readMode;

    // Audio
    AudioSource audio;
    Source audioSource;
    float audioCurrentTime;

    float audioElapsedTimer = 0.0f;

    LipSyncManager()
    {
        lipSync.SetBlendShapeWeight = delegate (int index, float weight)
        {
            characterMesh.SetBlendShapeWeight(index, weight);
        };

        lipSync.PopulateBlendshapeList = delegate ()
        {
            lipSync.blendShapes = new List<LipSyncGenerator.BlendShapes.BlendShape>();
            for (int i = 0; i < characterMesh.sharedMesh.blendShapeCount; i++)
            {
                string blendShapeName = characterMesh.sharedMesh.GetBlendShapeName(i);
                lipSync.blendShapes.Add(new LipSyncGenerator.BlendShapes.BlendShape(blendShapeName, i));
            }
        };
    }

    void Awake()
    {
        // check if there is a character available for lip-sync
        if (characterMesh == null)
        {
            Debug.Log("No character mesh is present");
            return;
        }

        lipSync.blendSpeed = blendSpeed;
        lipSync.PopulateBlendshapeList();

        // check readMode
        if (readMode.Equals(ReadMode.XML))
        {
            lipSync.PhonemeBlendShapeMapping(lipSync.pathManager.getDataPath("phonemeMapping.xml"));
            lipSync.DiphoneMapping(lipSync.pathManager.getDataPath("diphoneMapping.xml"));
            lipSync.EmotionBlendShapeMapping(lipSync.pathManager.getDataPath("emotionMapping.xml"));
        }

        lipSync.ListToDictionary(lipSync.staticVisemes);
        lipSync.ListToDictionary(lipSync.dynamicVisemes);
        lipSync.ListToDictionary(lipSync.emotions);
    }

    void Start()
    {
        // initialize 3D model
        InitializeModel();

        // phonemes
        lipSync.ReadPhonemes("phonemes.txt");
        lipSync.phonemeInformation = LipSyncGenerator.Speech.Synthesis.CoarticulationEnhancement.AddDynamicVisemes(lipSync.phonemeInformation, lipSync.diphonePhonemes);

        // emotions
        lipSync.currentEmotion = lipSync.emotions.FirstOrDefault(s => s.emotion.Equals(Emotion.Neutral));

        // audio
        Debug.Log("getcomponent audio source");
        audio = GetComponent<AudioSource>();
        audioSource = Source.Listener;

        if (lipSync.staticVisemes.Count == 0)
        {
            return;
        }

        // set current phoneme to first phoneme which corresponds to the phoneme Rest
        lipSync.SetCurrentPhoneme(LipSyncGenerator.Phonemes.Phoneme.Rest);
        if (lipSync.phonemeInformation.Count > 0)
        {
            lipSync.newCurrentPhoneme = lipSync.phonemeInformation[lipSync.currentIndex];
        }
        else
        {
            lipSync.newCurrentPhoneme = null;
        }

    }

    /// <summary>
    /// Plays audio from the generated audio source
    /// </summary>
    public void PlayAudio()
    {
        // change source to listener
        audioSource = Source.Speaker;

        //string audioUrl = Application.streamingAssetsPath + "/audio/audio.wav";
        string audioUrl = lipSync.pathManager.getAudioPath("audio.wav");
        lipSync.ReadPhonemes("phonemes.txt");
        lipSync.phonemeInformation = LipSyncGenerator.Speech.Synthesis.CoarticulationEnhancement.AddDynamicVisemes(lipSync.phonemeInformation, lipSync.diphonePhonemes);
        Debug.Log(audioUrl);
        StartCoroutine(WaitForAudio(audioUrl));
    }

    /// <summary>
    /// Couroutine that waits for the wav audio file to be loaded
    /// </summary>
    /// <param name="audioUrl">The url of the generated audio</param>
    /// <returns></returns>
    IEnumerator WaitForAudio(string audioUrl)
    {
        WWW audioRequest = new WWW("file://" + audioUrl);
        yield return audioRequest;

//        if (string.IsNullOrEmpty(audioRequest.error) == false)
//        {
//            print("ERROR: Audio Url Error:: " + audioRequest.error);
 //           yield break;
//        }

        GetComponent<AudioSource>().clip = audioRequest.GetAudioClip(false, false, AudioType.WAV);

        // change source to speaker
        if (String.IsNullOrEmpty(GetComponent<AudioSource>().clip.name))
        {
            audioSource = Source.Speaker;
        }
        else
        {
            audioSource = Source.Listener;
        }

        GetComponent<AudioSource>().PlayScheduled(0);

        if (!GetComponent<AudioSource>().isPlaying)
        {
            lipSync.currentIndex = 0;

            // change source to listener
            audioSource = Source.Listener;
        }
    }

    /// <summary>
    /// Fixed Update function which animates the character as long as the audio is playing
    /// </summary>
    void FixedUpdate()
    {
        if (GetComponent<AudioSource>().clip != null) // && audioSource.Equals(Source.Speaker))
        {
            if (GetComponent<AudioSource>().isPlaying)
            {

                // audio timer
                audioElapsedTimer += Time.deltaTime;
                lipSync.Animate(audioElapsedTimer);

                //// convert timeSamples to time
                audioCurrentTime = GetComponent<AudioSource>().timeSamples / (float)GetComponent<AudioSource>().clip.frequency; // audio current time
            }
            else
            {
                // re-initialize current index of phoneme
                lipSync.currentIndex = 0;
                lipSync.newCurrentPhoneme = lipSync.phonemeInformation[lipSync.currentIndex];

                // initialize emotions blendshapes
                lipSync.InitializeEmotions();

                // change source to listener
                audioSource = Source.Listener;

                // initialize audio timer
                audioElapsedTimer = 0.0f;
            }
        }

    }

    /// <summary>
    /// Initializes the 3D models' blendshapes to default values
    /// </summary>
    void InitializeModel()
    {
        for (int i = 0; i < characterMesh.sharedMesh.blendShapeCount; i++)
        {
            characterMesh.SetBlendShapeWeight(i, 0);
        }
    }
}
