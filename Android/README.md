# Chicken Taxi — standalone Android app (no Unity required)

This Android project packages the **offline HTML5 gameplay** in a minimal native Android WebView application. It is independently playable; it does not compile or embed the Unity prototype.

## Easiest APK: GitHub Actions

1. Go to [the repository Actions tab](https://github.com/g4paok1994-del/Chicken-Taxi/actions).
2. Select **Chicken Taxi Android APK - No Unity**.
3. Click **Run workflow** on the main branch, or inspect an automatic run.
4. Wait until the run has a **green success check**.
5. Download the **ChickenTaxi-Android-Playable-APK** artifact.
6. Extract the ZIP; install the contained `app-debug.apk` on an Android phone.

**If the run is red, there is no verified APK**. Open its build logs and address the error.

This build uses only Java/Gradle/Android SDK on GitHub-hosted Linux runners. **No Unity license, no Unity Secrets, and no password are needed**.

## Build locally

Prerequisites: Android Studio or command-line Android SDK with Android platform 35, JDK 17 and Gradle 8.9.

Open the `Android/` folder in Android Studio and run the `app` configuration, or from `Android/` run:

```bash
gradle :app:assembleDebug --no-daemon
```

The debug APK is written to `Android/app/build/outputs/apk/debug/app-debug.apk`.

## Browser preview

Open `Android/app/src/main/assets/index.html` in a modern browser. The game engine is `game.js` and CSS is `style.css`. Steering: screen buttons or arrow keys/A,D; Space to brake.

## Data use

The app contains no network access permission and does not integrate any remote API, tracking, AdMob or analytics. Coins, highest score and taxi upgrade level are saved locally in browser WebView storage. Do not confuse this debug build with the historical Unity release.

## QA checklist before wider distribution

- Open app on Android 8+ (min API 24) and confirm the PLAY menu is visible.
- Verify the touch controls steer and BRAKE slows the taxi.
- Pick up a yellow chicken; reach the green target before the timer expires.
- Confirm coins are rewarded, upgrade purchases work and progress survives app restart.
- Test pause, app background/foreground resume, Android back and device rotation lock.
- Verify APK integrity, Android version compatibility and device-specific UI.
- Production publication requires a signed AAB and a Play Console submission, with a reviewed privacy/data safety disclosure.
