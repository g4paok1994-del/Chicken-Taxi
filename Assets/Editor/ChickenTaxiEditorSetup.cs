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
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log("Chicken Taxi: generated the empty bootstrap scene.");
        }
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    [MenuItem("Chicken Taxi/Open Game Scene")]
    public static void OpenScene()
    {
        EnsureScene();
        EditorSceneManager.OpenScene(ScenePath);
    }

    [MenuItem("Chicken Taxi/Build Android Development APK")]
    public static void BuildAndroidApk()
    {
        EnsureScene();
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = false;
        PlayerSettings.companyName = "AL MEDIA";
        PlayerSettings.productName = "Chicken Taxi";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.almedia.chickentaxi");
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        Directory.CreateDirectory("Builds");
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = new[] { ScenePath },
            locationPathName = "Builds/ChickenTaxi-Development.apk",
            target = BuildTarget.Android,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        Debug.Log(report.summary.result == BuildResult.Succeeded
            ? "Chicken Taxi APK created: Builds/ChickenTaxi-Development.apk"
            : "Chicken Taxi APK failed: " + report.summary.result + ". Check the Unity Console.");
    }
}
#endif