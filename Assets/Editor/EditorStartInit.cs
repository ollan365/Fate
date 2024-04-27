using UnityEditor;
using UnityEditor.SceneManagement;

// Start�� �ƴ� ������ �۾��ϴٰ� Ȯ���Ϸ��� ���� �����ص� �ٷ� Start������ ��ŸƮ�ϰ� ����

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