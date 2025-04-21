using UnityEditor;
using UnityEngine;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine.Localization;

public class GoogleSheetLocalizationImporter : EditorWindow
{
    private const string urlFormat = "https://docs.google.com/spreadsheets/d/<sheetId>/export?format=csv&gid=<gid>";
    private const string sheetId = "1jXyT0KH0uI9uQ4lU8GUQrztj7IUX94A7z8RV2RH4t78";
    private const string gId = "128710286";
    private const string tableCollectionName = "LanguageTable";

     private static readonly Dictionary<string, string> langToLocaleCode = new()
    {
        { "ko", "ko-KR" },
        { "ko-kr", "ko-KR" },
        { "en", "en" },
        { "ja", "ja-JP" },
        { "ja-jp", "ja-JP" }
    };

    [MenuItem("Tools/Localization/Import from Google Sheet")]
    public static void ImportFromGoogleSheet()
    {
        var sheetUrl = urlFormat.Replace("<sheetId>", sheetId).Replace("<gid>", gId);
        Debug.Log($"📥 Load from {sheetUrl}");

        WebClient client = new();
        client.Encoding = Encoding.UTF8;

        string csvData;
        try
        {
            csvData = client.DownloadString(sheetUrl);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Google Sheet download failed: " + ex.Message);
            return;
        }

        var lines = csvData.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogError("❌ CSV contains no data.");
            return;
        }

        var headers = lines[0].Trim().Split(',');

        // CSV 헤더에서 언어 인덱스 추출
        Dictionary<string, int> langIndex = new();
        for (int i = 1; i < headers.Length; i++)
        {
            string langCode = headers[i].Trim().ToLower();
            if (langToLocaleCode.ContainsKey(langCode))
                langIndex[langCode] = i;
        }

        var collection = LocalizationEditorSettings.GetStringTableCollection(tableCollectionName);
        if (collection == null)
        {
            Debug.LogError($"❌ Collection not found: {tableCollectionName}");
            return;
        }

        int updated = 0;
        int added = 0;

        // 수정된 테이블 모음
        HashSet<StringTable> modifiedTables = new();

        // CSV 데이터 처리
        for (int row = 1; row < lines.Length; row++)
        {
            var cols = lines[row].Trim().Split(',');
            if (cols.Length < 2) continue;

            string key = cols[0].Trim();
            if (string.IsNullOrWhiteSpace(key)) continue;

            foreach (var lang in langIndex.Keys)
            {
                var localeId = new LocaleIdentifier(langToLocaleCode[lang]);
                var table = collection.GetTable(localeId) as StringTable;

                if (table == null)
                {
                    Debug.LogWarning($"⚠ Missing table for locale: {localeId.Code}");
                    continue;
                }

                string value = langIndex[lang] < cols.Length ? cols[langIndex[lang]].Trim() : "";
                if (string.IsNullOrEmpty(value)) continue;

                var entry = table.GetEntry(key);
                if (entry != null)
                {
                    entry.Value = value;
                    updated++;
                }
                else
                {
                    table.AddEntry(key, value);
                    added++;
                }

                modifiedTables.Add(table);
            }
        }

        // 변경된 테이블만 Dirty 처리
        foreach (var table in modifiedTables)
        {
            EditorUtility.SetDirty(table);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"✅ Localization import complete. Updated: {updated}, Added: {added}");
    }
}