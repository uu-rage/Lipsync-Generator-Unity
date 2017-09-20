# Lipsync-Generator-Unity

To run the demo directly, just extract "LipSync-Generator-Unity.7z" and run the executable file.

Dependency for this example:
- cerevoice SDK (get your own license)

How to setup your own project:
- Copy all files inside "Unity Source" into you Unity project folder.

- Unzip "StreamingAssets.zip" into Unity project folder.

- put your voice (*.voice) file (from cerevoice SDK) to "StreamingAssets/CereVoice" folders

- put your license (license.lic) file (from cerevoice SDK) to "StreamingAssets/CereVoice" folders

- Create an object inside Unity, name it "LipSync Manager".

- Drag "LipSyncManager" script into you "LipSync Manager" object.

- Drag "TextToSpeech" script into you "LipSync Manager" object.

- Make sure you have "Audio Source" component in "LipSync Manager" object.

- Set your "Character Mesh" parameter in "LipSyncManager" component to you own character mesh.
tips: make sure that your character have a blendshape information (character from Daz3D already have this information)

- Set your "Blend Speed" parameter in "LipSyncManager" component to "1.5".

- Set your "Read Mode" parameter in "LipSyncManager" component to "XML"
tips: this will read all XML inside "StreamingAssets" folders, but you can also manually set this parameter (change it to "inspector")

- Create a new "InputField" and "Button".

- Set your "Input Text" parameter in "TextToSpeech" component to your input field.

- Create two new events onClick for your button, and set it to 
"TextToSpeech"-"GenerateAudio"
"LipSyncManager"-"PlayAudio"

- done

