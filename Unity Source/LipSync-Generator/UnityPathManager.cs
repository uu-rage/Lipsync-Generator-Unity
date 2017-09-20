using UnityEngine;

public class UnityPathManager : LipSyncGenerator.Helpers.PathManager {
    private string getAssetPath()
    {
        return Application.streamingAssetsPath;
    }

    public string getAudioPath(string file)
    {
        return getAssetPath() + "/Audio/" + file;
    }

    public string getCereVoicePath(string file)
    {
        return getAssetPath() + "/CereVoice/" + file;
    }

    public string getDataPath(string file)
    {
        return getAssetPath() + "/" + file;
    }

    public string getOpenSmileConfigPath(string file)
    {
        return getAssetPath() + "/OpenSmile/" + file;
    }

    public string getOpenSmileDataPath(string file)
    {
        return getAssetPath() + "/OpenSmile/config/" + file;
    }

    public string getOpenSmilePath(string file)
    {
        return getAssetPath() + "/OpenSmile/data/" + file;
    }
}
