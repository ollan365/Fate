using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioOptimizer : EditorWindow
{
    [MenuItem("Tools/Optimize All Audio")]
    public static void OptimizeAllAudio() {
        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Resources" });
        
        int processed = 0;
        int total = audioGuids.Length;
        foreach (var guid in audioGuids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AudioImporter importer = AssetImporter.GetAtPath(path) as AudioImporter;

            if (importer == null) 
                continue;
            
            importer.forceToMono = false; // Keep stereo if original is stereo
            importer.loadInBackground = true; // Load in background for better performance
            importer.ambisonic = false; // Not ambisonic audio
                
            // Set compression settings for all platforms
            AudioImporterSampleSettings settings = new() {
                loadType = AudioClipLoadType.CompressedInMemory, // Compress in memory
                compressionFormat = AudioCompressionFormat.Vorbis, // Use Vorbis compression
                quality = 0.7f, // 70% quality (good balance of size vs quality)
                sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate, // Override sample rate
                sampleRateOverride = 44100 // 44.1kHz sample rate
            };
                
            importer.SetOverrideSampleSettings("Standalone", settings);
            importer.SetOverrideSampleSettings("Android", settings);
            importer.SetOverrideSampleSettings("iOS", settings);
            importer.SetOverrideSampleSettings("WebGL", settings);
                
            AssetDatabase.ImportAsset(path);
            processed++;
                
            EditorUtility.DisplayProgressBar("Optimizing Audio", 
                $"Processing {path} ({processed}/{total})", (float)processed / total);
        }
        
        EditorUtility.ClearProgressBar();
        Debug.Log($"Optimized {processed} audio files!");
    }
}