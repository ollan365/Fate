using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureOptimizer : EditorWindow
{
    [MenuItem("Tools/Optimize All Textures")]
    public static void OptimizeAllTextures()
    {
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources" });
        
        int processed = 0;
        int total = textureGuids.Length;
        foreach (var t in textureGuids) {
            string path = AssetDatabase.GUIDToAssetPath(t);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;
            
            importer.crunchedCompression = false;
            importer.maxTextureSize = 1024;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
                
            TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
            defaultSettings.format = TextureImporterFormat.RGBA32;
            defaultSettings.maxTextureSize = 1024;
            defaultSettings.compressionQuality = 100;
            defaultSettings.crunchedCompression = false;
                
            importer.SetPlatformTextureSettings(defaultSettings);
            AssetDatabase.ImportAsset(path);
            processed++;
                
            EditorUtility.DisplayProgressBar("Optimizing Textures", 
                $"Processing {path} ({processed}/{total})", (float)processed / total);
        }
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"Optimized {processed} textures!");
    }
}