# Chicken Taxi: building and testing Android

## Prerequisites
- Unity Hub and Unity **2022.3 LTS**
- Unity Android Build Support + OpenJDK + Android SDK & NDK Tools installed through Unity Hub
- A development machine with sufficient storage
- Android phone with USB debugging enabled (or a compatible emulator)

## Open in Unity
1. Download or clone the repository, then point Unity Hub to the folder containing Assets/, Packages/ and ProjectSettings/.
2. Let Unity finish importing assets/packages. The editor auto-creates Assets/Scenes/ChickenTaxi.unity if it does not exist.
3. Choose **Chicken Taxi > Open Game Scene**, then press Play.
4. Check the Console for C# compilation errors. If there are errors, resolve them before attempting a build.

## Build APK
1. Choose **Chicken Taxi > Build Android Development APK**.
2. The editor switches target platform to Android and configures IL2CPP, ARM64, Android API 24 minimum.
3. On success find Builds/ChickenTaxi-Development.apk.
4. Install using Unity Build & Run, or adb install -r Builds/ChickenTaxi-Development.apk.
5. Test pickups, deliveries, timer expiry, upgrades, pause, keyboard and touch steering, back button and resume.

## Important release notes
- This repository does **not** contain a tested APK yet.
- Google Mobile Ads/AdMob is not part of this minimal build. Integrate it separately after gameplay tests and implement a compliant consent flow before serving ads.
- A Google Play release additionally requires signing (keystore), an AAB, Play Console listing/data safety declarations, and testing.
- Do not treat a GitHub source ZIP as an installable Android application.
