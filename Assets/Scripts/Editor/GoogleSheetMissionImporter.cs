using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

public static class GoogleSheetMissionImporter
{
    private const string sheetId = "1jXyT0KH0uI9uQ4lU8GUQrztj7IUX94A7z8RV2RH4t78"; // 👈 실제 시트 ID로 교체

    private static readonly Dictionary<MissionType, string> gidMap = new()
    {
        { MissionType.Achievement, "0" }, // 👈 실제 GID로 교체
        { MissionType.Daily,       "330045768" },
        { MissionType.Event,       "1747749154" }
    };

    private const string savePath = "Assets/Resources/Data/Missions/";

    [MenuItem("Tools/Missions/Import Missions from Google Sheet")]
    public static void ImportAllMissions()
    {
        foreach (var entry in gidMap)
        {
            ImportMissionTypeFromSheet(entry.Key, entry.Value);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ All mission types imported.");
    }

    private static void ImportMissionTypeFromSheet(MissionType type, string gid)
    {
        string url = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";
        Debug.Log($"📥 Downloading {type} from {url}");

        WebClient client = new();
        client.Encoding = Encoding.UTF8;

        string csv;
        try
        {
            csv = client.DownloadString(url);
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Failed to download {type} data: {ex.Message}");
            return;
        }

        string[] lines = csv.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogWarning($"⚠ No data found in {type} sheet.");
            return;
        }

        var headers = lines[0].Trim().Split(',');
        Dictionary<string, int> col = new();
        for (int i = 0; i < headers.Length; i++)
            col[headers[i].Trim()] = i;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            if (values.Length < 2 || string.IsNullOrWhiteSpace(values[0])) continue;

            var asset = ScriptableObject.CreateInstance<MissionData>();
            asset.id = values[col["id"]];
            asset.missionType = type;
            asset.title = values[col["title"]];
            asset.description = values[col["description"]];
            asset.conditionType = Enum.Parse<MissionConditionType>(values[col["condition"]]);
            asset.evaluateType = Enum.Parse<MissionEvaluateType>(values[col["evaluate"]]);
            asset.complexityType = Enum.Parse<MissionComplexityType>(values[col["complexityType"]]);
            asset.requiredValue = int.Parse(values[col["required"]]);
            asset.rewardType = Enum.Parse<CurrencyType>(values[col["rewardType"]]);
            asset.rewardAmount = int.Parse(values[col["rewardAmount"]]);
            
            if (type == MissionType.Event)
            {
                asset.isTimeLimited = bool.Parse(values[col["isTimeLimited"]]);
                asset.startTime = DateTime.Parse(values[col["startTime"]]);
                asset.endTime = DateTime.Parse(values[col["endTime"]]);
            }
            else
            {
                asset.isTimeLimited = false;
                asset.startTime = DateTime.MinValue;
                asset.endTime = DateTime.MaxValue;
            }

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            string prefix = type.ToString();
            string path = $"{savePath}{prefix}_{asset.id}.asset";

            if (File.Exists(path))
            {
                var oldAsset = AssetDatabase.LoadAssetAtPath<MissionData>(path);
                if (oldAsset != null)
                {
                    EditorUtility.CopySerialized(asset, oldAsset);
                    EditorUtility.SetDirty(oldAsset);
                    Debug.Log($"♻️ Updated: {path}");
                }
                else
                {
                    Debug.LogWarning($"⚠ Could not load existing asset: {path}");
                }
            }
            else
            {
                AssetDatabase.CreateAsset(asset, path);
                Debug.Log($"✅ Created: {path}");
            }
        }
    }
}
