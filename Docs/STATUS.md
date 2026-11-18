# Chicken Taxi — Project and Android CI Status

**Last reviewed:** November 18, 2026

## Implemented in GitHub

- Runtime-generated 2D prototype with chicken pickups, deliveries, score/coins and touch steering.
- Unity 2022.3 project structure, UGUI manifest, editor scene/bootstrap and local Android build entry point.
- GitHub Actions workflow: [Build Chicken Taxi Android APK](../.github/workflows/android-apk.yml).
- Development APK verification and artifact upload, executed after a successful Unity build.
- Explicit build failure propagation and setup instructions: [GITHUB_ACTIONS_APK.md](GITHUB_ACTIONS_APK.md).

## Observed CI result

GitHub Actions [run #44309939927](https://github.com/g4paok1994-del/Chicken-Taxi/actions/runs/44309939927) failed on November 18, 2026 at the **Check Unity activation** step. The project structure check passed. All tested Unity activation environment variables were empty; Unity itself was not launched. Therefore **there is not yet a verified APK**, and Unity compilation/device operation remains untested.

## Action required by repository owner

Configure a supported Unity license/activation method in GitHub repository **Settings → Secrets and variables → Actions**, then rerun **Build Chicken Taxi Android APK**. See [GITHUB_ACTIONS_APK.md](GITHUB_ACTIONS_APK.md) and the GameCI activation instructions. Do not publish credentials.

## Additional work before production

- Resolve any Unity compilation/build errors identified by the first activated CI run.
- Install and test the development APK on at least one Android phone.
- Add production graphics and gameplay polish.
- AdMob and consent flow are not implemented in this simplified prototype.
- Signing, production AAB, compliant privacy/data safety declarations and Play Store submission are separate tasks.
