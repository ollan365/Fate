using UnityEditor;
using UnityEditor.SceneManagement;

// Start가 아닌 씬에서 작업하다가 확인하려고 게임 시작해도 바로 Start씬에서 스타트하게 해줌

[InitializeOnLoad]
public class EditorStartInit
{
    static EditorStartInit()
    {
        var pathOfFirstScene = EditorBuildSettings.scenes[0].path;
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfFirstScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }
}