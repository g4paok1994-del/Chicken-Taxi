# 🐔🚕 Chicken Taxi — Android Game

**Play the Chicken Taxi prototype on Android without installing Unity.**

The repository now contains **two different implementations**:

| Folder | Implementation | Build requirement |
|---|---|---|
| `Android/` | Standalone playable HTML5 game inside an Android WebView | GitHub Actions and Android Gradle; **no Unity license** |
| `Assets/`, `Packages/`, `ProjectSettings/` | Original Unity C# prototype | Unity 2022.3 and a valid Unity activation |

## Download an Android APK (recommended)

1. Open **[GitHub Actions](https://github.com/g4paok1994-del/Chicken-Taxi/actions)**.
2. Select **Chicken Taxi Android APK - No Unity**.
3. Choose **Run workflow** on `main`, or open a successful automatic run.
4. When the run shows a **green tick**, download the artifact **ChickenTaxi-Android-Playable-APK**.
5. Extract the downloaded ZIP to find `app-debug.apk`.
6. Transfer `app-debug.apk` to Android and install it from your file manager, allowing installation from that source when Android asks.

**Important:** An APK exists only when GitHub Actions finishes its real Android build successfully. The GitHub release tagged `v1.0.0` is an older source snapshot, not an Android installer.

Detailed instructions: **[Android/README.md](Android/README.md)**.

## Play online (GitHub Pages)

**The online URL is not live until GitHub Pages is enabled.** To publish the playable HTML5 version, open [Settings → Pages](https://github.com/g4paok1994-del/Chicken-Taxi/settings/pages). Set **Source → Deploy from a branch**, **Branch → main**, **Folder → /docs**, and click **Save**. GitHub then publishes the site at [https://g4paok1994-del.github.io/Chicken-Taxi/](https://g4paok1994-del.github.io/Chicken-Taxi/). The three required web files are already committed in the lowercase `docs/` folder; no Unity, APK or secret is needed.

The earlier GitHub Actions Pages deployment failed because Pages was not enabled. Do not treat the site URL as verified live until GitHub displays a publication status or the browser loads the game. See [Docs/PLAY_ONLINE.md](Docs/PLAY_ONLINE.md).

## Play in a browser

The same HTML5 game can run in a modern desktop/mobile browser using `Android/app/src/main/assets/index.html`. It needs no internet connection, no API keys, and no Unity license.

## Gameplay in the Android version

- Drive a stylized yellow taxi with left/right touch buttons; hold BRAKE for tighter turning
- Find yellow chicken pickup markers and deliver to green destinations
- Random endless missions with timed deliveries
- Earn coins, build score, and purchase persistent taxi upgrades
- Pause/resume, navigate with a directional arrow, and hear simple effects
- Offline play; no accounts or ads in this prototype

## Android build source

- `Android/app/src/main/java/com/chickentaxi/game/MainActivity.java` — native Android WebView host
- `Android/app/src/main/assets/index.html`, `style.css`, `game.js` — offline game
- `Android/app/build.gradle` — Android project configuration
- `.github/workflows/android-native-apk.yml` — Android APK CI without proprietary Unity activation

## Unity prototype

Original Unity code remains intact. Its separate [Unity CI workflow](.github/workflows/android-apk.yml) requires a Unity license. For that route see [Docs/GITHUB_ACTIONS_APK.md](Docs/GITHUB_ACTIONS_APK.md).

## Product readiness

This is **prototype/test software**. A GitHub Actions debug APK is not equivalent to a production-signed Google Play release. Before a store launch, implement production signing/AAB, complete on-device QA, add a final app icon and branding as needed, review privacy/data safety declarations, and implement AdMob/consent if ads will be enabled.

**No AdMob, advertising network, analytics SDK, or network permission is included in the standalone Android prototype.**

## License

All rights reserved unless the project owner grants a separate license.
