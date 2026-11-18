# GitHub Actions — automatic Android APK

The workflow at `.github/workflows/android-apk.yml` builds a **Unity development APK** via GameCI and uploads it as a downloadable GitHub Actions artifact.

## One-time setup (required)

1. Open repository **Settings → Secrets and variables → Actions**.
2. Follow the official [GameCI activation instructions](https://game.ci/docs/github/activation) for your Unity license type. Add the supported repository secrets:
   - `UNITY_LICENSE` (contents of a Unity license file), **or** a supported `UNITY_EMAIL` / `UNITY_PASSWORD` activation method.
   - `UNITY_SERIAL` only if your license type needs it.
3. Do **not** add license or login credentials to a public file, commit, issue, or message.
4. If your license type cannot be activated by GameCI on a hosted runner, use a licensed self-hosted runner or a local Unity build instead.

## Run the workflow

1. Open [Actions](https://github.com/g4paok1994-del/Chicken-Taxi/actions).
2. Choose **Build Chicken Taxi Android APK**.
3. Click **Run workflow** on branch `main`.
4. Open the new run and watch the build steps.
5. On **success** (green tick), find the **Artifacts** section of that run.
6. Download `ChickenTaxi-Android-Development-APK` and extract the ZIP. It should contain `ChickenTaxi-Development.apk`.
7. Install the APK on an Android phone that permits installation from this source. Test controls, pickups, delivery missions, coins, pause/resume and upgrades.

The workflow also runs when relevant source, package, project-settings or workflow files are pushed to `main`.

## What the automation does

- Verifies a minimal Unity project structure.
- Checks whether Unity activation secrets were configured (never logs their values).
- Runs GameCI Unity Builder with Unity version taken from `ProjectSettings/ProjectVersion.txt`.
- Executes `ChickenTaxiEditorSetup.BuildAndroidApk` in headless Unity. The editor script creates the empty scene if necessary, then builds the APK.
- Throws an error if the Unity build fails or no APK is written.
- Verifies that the APK exists and has a readable ZIP structure.
- Uploads the APK as an Actions artifact for 14 days.

## Troubleshooting

- **Unity activation missing:** Configure GitHub Actions secrets above.
- **Unity activation failed:** Verify your license type and follow the GameCI activation guide.
- **Editor version / container unavailable:** Choose an available Unity 2022.3 patch and keep the project version in sync.
- **C# compile error:** Inspect the **Build Android APK (GameCI)** job logs, then fix the C# file named in the log.
- **APK not found:** Inspect the Unity build report; the editor build method must create `Builds/ChickenTaxi-Development.apk`.
- **Workflow unavailable:** Check repository Actions settings and third-party action permissions.

## Project audit, November 18, 2026

**Present in repository:** Unity version reference, UGUI package manifest, self-bootstrapping C# endless gameplay, runtime-generated menus, driving controls, missions, coins/upgrades, editor script and art/audio placeholders.

**Addressed here:** missing CI workflow, headless build invocation, explicit failure handling, artifact verification and download instructions.

**Still outstanding before declaring the app ready:** compatible Unity license configured in GitHub Actions; a green, real Android build; install and device testing; production signing (keystore); Play Store submission. **AdMob, UMP consent, verified tilt control, traffic AI and weather are not part of the current minimal prototype.**

The historical GitHub `v1.0.0` release is a source-code snapshot, **not a playable Android APK**. Download the APK only from a successful Actions run.
