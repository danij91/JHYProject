using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LocalData {
    public enum EReturn {
        SUCCESS,
        NO_OBJECT,
        BINARY_ERROR,
        WRITING_ERROR,
    }

    public const string BEST_COUNT_KEY = "BEST_COUNT_KEY";
    public const string CHARACTER_INVEN_KEY = "CHARACTER_INVEN_KEY";
    public const string MAIN_CHARACTER_KEY = "MAIN_CHARACTER_KEY";

    private static LocalData instance = null;
    public static LocalData Instance {
        get {
            if (instance == null) {
                instance = new LocalData();
            }

            return instance;
        }
    }

    DataSet dataSet;

    public void Initialize() {
        // 캐시된 데이터가 없다면
        if (!LoadDataSetFromFile()) {
            Reset();
        }
    }

    public void Reset() { }

    private bool LoadDataSetFromFile() {
        string path = GetDataSetPath();

        if (System.IO.File.Exists(path)) {
            dataSet = LoadFromFile<DataSet>(path);
        }

        if (dataSet == null) {
            dataSet = new DataSet();
            return false;
        }

        return true;
    }

    private T LoadFromFile<T>(string inFilePath, string inSecretKey = null) {
        if (!System.IO.File.Exists(inFilePath)) {
            Debug.Log(inFilePath + "의 파일이 존재하지 않습니다.");
            return default(T);
        }

        try {
            FileStream fs = new FileStream(inFilePath, FileMode.Open);
            byte[] byteArr = new byte[fs.Length];
            fs.Read(byteArr, 0, System.Convert.ToInt32(fs.Length));
            fs.Close();

            byte[] result;

            result = byteArr;


            MemoryStream ms = new MemoryStream(result);
            BinaryFormatter f = new BinaryFormatter();
            return (T) f.Deserialize(ms);
        }
        catch (System.Exception e) {
            Debug.Log(e.ToString());
            return default(T);
        }
    }

    public void SetKey<T>(string key, object value) {
        if (dataSet == null) {
            Initialize();
        }

        dataSet.Set<T>(key, value);
        SaveCachedDataSet();
    }

    public void RemoveKey<T>(string key) {
        if (dataSet == null) {
            Initialize();
        }

        if (dataSet.Has<T>(key)) {
            dataSet.Remove<T>(key);
        }

        SaveCachedDataSet();
    }

    public T GetKey<T>(string key) {
        if (dataSet == null) {
            Initialize();
        }

        return dataSet.Get<T>(key);
    }

    public bool TryGetValue<T>(string key, out T result) {
        if (dataSet.Has<T>(key)) {
            result = GetKey<T>(key);
            return true;
        } else {
            result = default(T);
            return false;
        }
    }

    public void SaveCachedDataSet() {
        EReturn eMessage = SaveAsFile(dataSet, GetDataSetPath());
        if (eMessage != EReturn.SUCCESS)
            Debug.LogError(eMessage);
    }

    private EReturn SaveAsFile(object inObject, string inFilePath) {
        if (inObject == null) {
            return EReturn.NO_OBJECT;
        }

        string folderPath = inFilePath.Substring(0, inFilePath.LastIndexOf("/"));
        if (!System.IO.Directory.Exists(folderPath)) {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        MemoryStream ms = new MemoryStream();
        byte[] byteArr = null;
        try {
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(ms, inObject);
            byteArr = ms.ToArray();
            ms.Close();
        }
        catch (System.Exception e) {
            ms.Close();
            byteArr = null;
            Debug.Log("<color=#ff0000>" + e.Message + "</color>");

            return EReturn.BINARY_ERROR;
        }

        FileStream fs = new FileStream(inFilePath, FileMode.Create);
        try {
            fs.Write(byteArr, 0, byteArr.Length);
            fs.Close();
        }
        catch (System.Exception) {
            fs.Close();
            return EReturn.WRITING_ERROR;
        }

        return EReturn.SUCCESS;
    }

    public string GetDataSetPath() {
        return $"{Application.persistentDataPath}.dat";
    }

    public void Clear() {
        if (dataSet != null) {
            dataSet.Clear();
            SaveCachedDataSet();
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/LocalData/Delete Caching Data")]
    public static void OnMenuDeleteCachingData() {
        if (Directory.Exists(Application.persistentDataPath)) Directory.Delete(Application.persistentDataPath, true);
    }

    [MenuItem("Tools/LocalData/Clear BestCount")]
    public static void LoaclDataClear_BestCount() {
        Instance.Initialize();
        Instance.RemoveKey<int>(BEST_COUNT_KEY);
    }

    [MenuItem("Tools/LocalData/Clear CharacterInven")]
    public static void LoaclDataClear_CharacterInven() {
        Instance.Initialize();
        Instance.RemoveKey<int>(CHARACTER_INVEN_KEY);
        Instance.RemoveKey<int>(MAIN_CHARACTER_KEY);
    }

    [MenuItem("Tools/LocalData/Clear PlayerPrefs")]
    public static void LocalDataClear_PlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }
#endif
}


public static class LocalDataHelper {
    public static int GetMainCharacter() {
        return LocalData.Instance.GetKey<int>(LocalData.MAIN_CHARACTER_KEY);
    }

    public static void SaveMainCharacter(int intType) {
        LocalData.Instance.SetKey<int>(LocalData.MAIN_CHARACTER_KEY, intType);
    }

    public static int GetLanguage() {
        return PlayerPrefs.GetInt("language", LocalizationManager.Instance.GetCurrentLanguage());
    }
    
    public static void SetLanguage() {
        PlayerPrefs.SetInt("language", LocalizationManager.Instance.GetCurrentLanguage());
    }
}
