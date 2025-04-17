// Editor/CharacterThumbnailCapturer.cs
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class CharacterThumbnailCapturer : EditorWindow {
    private CharacterDatabase characterDatabase;
    private Camera thumbnailCamera;
    private RenderTexture renderTexture;
    private int resolution = 512;
    private string outputPath = "Assets/Thumbnails";

    [MenuItem("Tools/Character Thumbnail Capturer")]
    public static void ShowWindow() {
        GetWindow<CharacterThumbnailCapturer>("Thumbnail Capturer");
    }

    private void OnGUI() {
        GUILayout.Label("Thumbnail Settings", EditorStyles.boldLabel);

        characterDatabase = (CharacterDatabase)EditorGUILayout.ObjectField("Character Database", characterDatabase, typeof(CharacterDatabase), false);
        thumbnailCamera = (Camera)EditorGUILayout.ObjectField("Capture Camera", thumbnailCamera, typeof(Camera), true);
        resolution = EditorGUILayout.IntField("Resolution", resolution);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        if (GUILayout.Button("Capture Thumbnails")) {
            if (characterDatabase == null || thumbnailCamera == null) {
                Debug.LogError("Please assign CharacterDatabase and Camera.");
                return;
            }
            CaptureAllThumbnails();
        }
    }

    private void CaptureAllThumbnails() {
        renderTexture = new RenderTexture(resolution, resolution, 24);
        thumbnailCamera.targetTexture = renderTexture;

        if (!Directory.Exists(outputPath)) {
            Directory.CreateDirectory(outputPath);
        }

        foreach (var character in characterDatabase.GetAll()) {
            if (character.modelPrefab == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(character.modelPrefab);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.Euler(0, 180, 0);

            thumbnailCamera.Render();

            RenderTexture.active = renderTexture;
            Texture2D screenshot = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            screenshot.Apply();

            byte[] bytes = screenshot.EncodeToPNG();
            string filename = Path.Combine(outputPath, character.characterId + ".png");
            File.WriteAllBytes(filename, bytes);

            DestroyImmediate(instance);
        }

        RenderTexture.active = null;
        thumbnailCamera.targetTexture = null;
        renderTexture.Release();

        AssetDatabase.Refresh();
        Debug.Log("✅ Thumbnail capture completed.");
    }
}
