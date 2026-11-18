# Chicken Taxi — current build status

The project has **two distinct build paths**.

## Preferred path — Android without Unity

- Source: `Android/`, standalone WebView game and Gradle build.
- Workflow: `.github/workflows/android-native-apk.yml`.
- No Unity activation or `UNITY_LICENSE` is needed.
- Successful workflow runs upload an installable debug APK artifact named `ChickenTaxi-Android-Playable-APK`.
- Verify the actual run status in [GitHub Actions](https://github.com/g4paok1994-del/Chicken-Taxi/actions); do not treat source commits as evidence of a successful APK build.
- See `Android/README.md` for installation and QA instructions.

## Optional Unity path

- Source: `Assets/`, `Packages/`, `ProjectSettings/`.
- Workflow: `.github/workflows/android-apk.yml`.
- Run #44309939927 failed at **Check Unity activation** on November 18, 2026 because the required Unity activation secrets were absent. The structure check passed, but Unity did not run.
- Unity route remains available for later improvements, provided a compatible license is configured.

## Remaining before production

On-device testing, polished assets, final branding, production signing and AAB, privacy/data safety review, optional AdMob integration and an appropriate user consent flow. The Android build is a **prototype**, not a certified Google Play release.
