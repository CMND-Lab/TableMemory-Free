# VR Table Memory Task

This project is for a location updating recognition task, implemented in Unity, using UXF for data collection: https://github.com/immersivecognition/unity-experiment-framework.
On each block, an collection of 5 objects are presented on a table, and half of these are interacted with. After moving locations or remaining stationary, a series of OLD/NEW recognition tests are completed.

## Requirements
- SteamVR & compatible headset
- Unity Hub & Unity v2021.3.45f

## Getting Started

1. Using Git bash or a Command Prompt window, clone the repo:
```
git clone https://github.com/CMND-Lab/VR-SingletonCapture_ViveProEye_OSF.git
```

2. Import the project into Unity Hub & open it. NOTE: If the scene is not loaded, navigate to Assets/Scenes and drag the "FreeOnly" scene into the hierarchy window

3. If SteamVR doesn't open automatically, open it

4. Run the project using the Play button at the top of the Unity window, then fill in the information in the UXF startup window
   - After the data directory has been selected once, it will be automatically loaded in future sessions
   - The dice button can be used to generate a random participant name

5. After cliking "Start" in the UXF startup window, the participant can use the controller to progress through the instructions
