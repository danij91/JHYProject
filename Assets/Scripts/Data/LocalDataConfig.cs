using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class LocalDataConfig : ScriptableObject
{
    private static LocalDataConfig _instance;
    public static LocalDataConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load(nameof(LocalDataConfig)) as LocalDataConfig;
#if UNITY_EDITOR

                if (_instance == null)
                {
                    _instance = CreateInstance();
                }
#endif
            }

            return _instance;
        }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/LocalDataConfig/Create Instance")]
    public static LocalDataConfig CreateInstance()
    {
        if (_instance == null)
        {
            _instance = CreateInstance<LocalDataConfig>();

            if (System.IO.Directory.Exists($"{Application.dataPath}/LocalData/Resources") == false)
            {
                System.IO.Directory.CreateDirectory($"{Application.dataPath}/LocalData/Resources");
            }

            AssetDatabase.CreateAsset(_instance, $"Assets/LocalData/Resources/{nameof(LocalDataConfig)}.asset");
        }

        Selection.activeObject = _instance;
        return _instance;
    }
#endif

    [Header("[Start TitleScene Forced]")]
    public bool IsStartTitleScene = false;

    [Header("[Debug Log Level]")]
    public EConfig.ELogLevel LogLevel = EConfig.ELogLevel.None;
}