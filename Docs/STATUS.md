# Chicken Taxi — verified Android build status

**Updated November 18, 2026**

## Android APK: GitHub Actions build SUCCEEDED

The standalone Android implementation in `Android/` has a verified successful GitHub Actions run:

- **Workflow:** `Chicken Taxi Android APK - No Unity` at `.github/workflows/android-native-apk.yml`
- **Successful run:** https://github.com/g4paok1994-del/Chicken-Taxi/actions/runs/44311531853
- **Artifact:** `ChickenTaxi-Android-Playable-APK` (artifact ID `14429789886`)
- **Inside artifact ZIP:** `app-debug.apk`
- **Validation:** JavaScript syntax, Android SDK setup, Gradle `assembleDebug`, APK ZIP-format integrity and artifact upload all passed.
- **Activation/secrets:** No Unity license, Unity account, keystore or GitHub Secrets required for this signed debug APK.

**Important:** This is an Android *debug build*. The GitHub CI success proves that the APK can be built and packaged, not that it has been play-tested on real phones. The app is an HTML5 game running inside a native Android WebView and is distinct from the Unity prototype.

## Installation

Visit the successful Actions run, download the artifact named `ChickenTaxi-Android-Playable-APK`, extract its ZIP, and install `app-debug.apk` on your Android device. The artifact may expire after 30 days; rerun the workflow to generate a new download.

## Other build path: original Unity prototype

The Unity implementation remains under `Assets/`, `Packages/`, and `ProjectSettings/`. Its separate workflow `android-apk.yml` needs compatible Unity activation credentials. The Unity job has **not** succeeded because activation secrets are missing. For playing the standalone Android game, **ignore the Unity workflow**.

## Before production release

Test the installed APK on one or more actual Android devices; confirm the touch controls, chicken pickups, missions, sound, progress persistence, pause/resume and app launch. Google Play distribution requires production app signing, AAB, Play Console review, and truthful privacy/data safety information. **AdMob is not integrated** in the present standalone app.
