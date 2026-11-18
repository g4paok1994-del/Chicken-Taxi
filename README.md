# 🐔🚕 Chicken Taxi — Android Game Prototype

An **endless, top-down 2D taxi game** where chickens are your passengers. Drive, pick up chickens, deliver them to destinations, earn coins and improve your taxi.

## Current status

**Playable Unity prototype source, not a compiled Android APK.** The game bootstraps all objects and UI at runtime and requires no imported visual assets to start. The original graphics and audio remain in this repository as optional reference assets.

Implemented and available in \`Assets/Scripts/ChickenTaxiGame.cs\`:
- Auto-driving taxi with large left/right touch controls and desktop arrow/A-D keys
- A repeatable, procedurally generated road-grid appearance
- Yellow chicken pickup and green delivery targets
- Random passenger mission descriptions, delivery timers and rewards
- Persistent coins and taxi speed upgrades through PlayerPrefs
- Pause, resume, main menu, directional indicator and simple generated sound effects

**Not yet included:** a tested signed APK/AAB, Google AdMob, production privacy consent, final art, true traffic AI, Google Play Store publication. Earlier ZIPs and documentation may describe planned features rather than implemented ones.

## Open and play in Unity

1. Install **Unity 2022.3 LTS** with Android Build Support, SDK/NDK and OpenJDK.
2. Clone/download the repository and open the **repository root** in Unity Hub as a project.
3. Unity imports the packages and the editor utility generates \`Assets/Scenes/ChickenTaxi.unity\` automatically.
4. Choose **Chicken Taxi → Open Game Scene** (or open the generated scene).
5. Press **Play** in the Unity Editor. The runtime script generates the taxi, world and UI.

If you're using Unity 6 or later, use a compatible version of UI/Text and enable the legacy Input Manager, or adapt the input code.

## Build a test Android APK

In Unity choose **Chicken Taxi → Build Android Development APK**. A successfully completed build writes \`Builds/ChickenTaxi-Development.apk\`. If a missing Android module or build error appears, read the Unity Console and \`Docs/ANDROID_BUILD.md\`.

This script is a **development APK**, not a store release. The project has not been compiled or tested on a physical Android phone by the authoring assistant.

## Files

- \`Assets/Scripts/ChickenTaxiGame.cs\`: endless game runtime
- \`Assets/Editor/ChickenTaxiEditorSetup.cs\`: editor scene setup and Android development build command
- \`Assets/Art\`, \`Assets/Audio\`, \`Assets/UI\`: optional original placeholder assets
- \`Docs/ANDROID_BUILD.md\`: build/testing instructions
- \`Docs/privacy_policy.html\`: historical draft; review and update before monetization

## Versioning

The existing GitHub \`v1.0.0\` release is an **earlier source snapshot**, not an installable APK. See the \`main\` branch for the newer prototype.

## License

No license has yet been granted. All rights reserved unless a separate license is added by the repository owner.
