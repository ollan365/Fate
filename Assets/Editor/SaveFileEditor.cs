using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveFileEditor : EditorWindow
{
    private string saveFilePath;
    private bool saveFileExists = false;

    [MenuItem("Tools/Save File Manager")]
    public static void ShowWindow()
    {
        SaveFileEditor window = GetWindow<SaveFileEditor>("Save File Manager");
        window.minSize = new Vector2(300, 120);
        window.Show();
    }

    private void OnEnable()
    {
        UpdateSaveFileInfo();
    }

    private void OnGUI()
    {
        GUILayout.Label("Save File Manager", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Display save file path
        GUILayout.Label("Save File Path:", EditorStyles.label);
        EditorGUILayout.SelectableLabel(saveFilePath, EditorStyles.textField, GUILayout.Height(20));
        
        GUILayout.Space(5);

        // Display save file status
        string statusText = saveFileExists ? "Save file exists" : "No save file found";
        Color statusColor = saveFileExists ? Color.green : Color.red;
        
        GUI.color = statusColor;
        GUILayout.Label(statusText, EditorStyles.boldLabel);
        GUI.color = Color.white;

        GUILayout.Space(10);

        // Delete save file button
        GUI.enabled = saveFileExists;
        if (GUILayout.Button("Delete Save File", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Delete Save File", 
                "Are you sure you want to delete the save file?\nThis action cannot be undone.", 
                "Delete", "Cancel"))
            {
                DeleteSaveFile();
            }
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        // Additional info
        GUILayout.Label("Note: This will permanently delete the current save file.", EditorStyles.helpBox);
    }

    private void UpdateSaveFileInfo()
    {
        saveFilePath = Application.persistentDataPath + "/GameData.json";
        saveFileExists = File.Exists(saveFilePath);
    }

    private void DeleteSaveFile()
    {
        try
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted successfully: " + saveFilePath);
                
                // Also delete backup file if it exists
                string backupPath = saveFilePath + ".bak";
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                    Debug.Log("Backup file also deleted: " + backupPath);
                }
                
                UpdateSaveFileInfo();
                EditorUtility.DisplayDialog("Success", "Save file has been deleted successfully!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No Save File", "No save file found to delete.", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to delete save file: " + e.Message);
            EditorUtility.DisplayDialog("Error", "Failed to delete save file: " + e.Message, "OK");
        }
    }
}
