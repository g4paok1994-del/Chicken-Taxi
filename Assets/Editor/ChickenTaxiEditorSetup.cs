#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class ChickenTaxiEditorSetup
{
    const string ScenePath = "Assets/Scenes/ChickenTaxi.unity";
    const string ApkPath = "Builds/ChickenTaxi-Development.apk";

    static ChickenTaxiEditorSetup()
    {
        EditorApplication.delayCall += EnsureScene;
    }

    static void EnsureScene()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        if (!File.Exists(ScenePath))
        {
            Directory.CreateDirectory("Assets/Scenes");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
                throw new BuildFailedException("Cannot save Chicken Taxi scene.");
            Debug.Log("Chicken Taxi: created bootstrap scene " + ScenePath);
        }
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    [MenuItem("Chicken Taxi/Open Game Scene")]
    public static void OpenScene()
    {
        EnsureScene();
        EditorSceneManager.OpenScene(ScenePath);
    }

    // Used by GitHub Actions with buildMethod: ChickenTaxiEditorSetup.BuildAndroidApk
    // and locally from the Unity menu.
    [MenuItem("Chicken Taxi/Build Android Development APK")]
    public static void BuildAndroidApk()
    {
        EnsureScene();
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            throw new BuildFailedException("Switch to Android failed. Install Android Build Support.");

        EditorUserBuildSettings.buildAppBundle = false;
        PlayerSettings.companyName = "AL MEDIA";
        PlayerSettings.productName = "Chicken Taxi";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.almedia.chickentaxi");
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        Directory.CreateDirectory("Builds");

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = ApkPath,
            target = BuildTarget.Android,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report == null || report.summary.result != BuildResult.Succeeded)
        {
            string outcome = report == null ? "no build report" : report.summary.result.ToString();
            throw new BuildFailedException("Android build failed: " + outcome);
        }
        if (!File.Exists(ApkPath) || new FileInfo(ApkPath).Length == 0)
            throw new BuildFailedException("The Unity build succeeded but APK is missing: " + ApkPath);

        Debug.Log("Chicken Taxi development APK created: " + ApkPath);
    }
}
#endif
