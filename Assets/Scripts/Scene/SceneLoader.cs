using System;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public sealed class EScene
{
    public const string TITLE = "TitleScene";
    public const string LOBBY = "LobbyScene";
    public const string INGAME = "InGameScene";

    public static bool IsLobbyScene() => SceneManager.GetActiveScene().name == LOBBY;
    public static bool IsInGameScene() => SceneManager.GetActiveScene().name == INGAME;
    public static bool IsActiveScene(string sceneName) => SceneManager.GetActiveScene().name == sceneName;
    public static string GetActiveScene() => SceneManager.GetActiveScene().name;
}


public class SceneLoader : Singleton<SceneLoader>
{
    private string CurrentSceneType { get; set; }
    private string PrevSceneType { get; set; }

    private bool _isInitalize = false;


    public void Initalize()
    {
        SceneManager.activeSceneChanged += OnChangedActiveScene;
        string activeSceneType = EScene.GetActiveScene();
        CurrentSceneType = activeSceneType;
        PrevSceneType = string.Empty;

        UIManager.Instance.Initialize();

        SceneBase sceneBase = GameObject.Find(activeSceneType).GetComponent<SceneBase>();
        sceneBase.OnStart();

        _isInitalize = true;
    }

    public bool IsInitalize()
    {
        return _isInitalize;
    }

    public async UniTask ChangeSceneAsync(string sceneType, bool isAdditive = false)
    {
        PrevSceneType = SceneManager.GetActiveScene().name;
        SceneBase currentSceneBase = GameObject.Find(CurrentSceneType).GetComponent<SceneBase>();
        currentSceneBase.OnClear();

        UIManager.Instance.CloseAll();

        Scene scene = SceneManager.GetSceneByName(sceneType);
        if (scene.name.IsNullOrEmpty())
        {
            await LoadAsyncScene(sceneType, isAdditive);
        }
        else
        {
            SceneManager.SetActiveScene(scene);
        }
        currentSceneBase.UpdateScene();
    }

    public void ChangeScene(string sceneType, bool isAdditive = false, Action<Scene> inCallback = null)
    {
        PrevSceneType = SceneManager.GetActiveScene().name;
        SceneBase currentSceneBase = GameObject.Find(CurrentSceneType).GetComponent<SceneBase>();
        currentSceneBase.OnClear();

        UIManager.Instance.CloseAll();

        Scene scene = SceneManager.GetSceneByName(sceneType);
        if (scene.name.IsNullOrEmpty())
        {
            StartCoroutine(LoadAsyncScene(sceneType, isAdditive));
        }
        else
        {
            SceneManager.SetActiveScene(scene);
        }
        currentSceneBase.UpdateScene();
        inCallback?.Invoke(scene);
    }

    IEnumerator LoadAsyncScene(string sceneType, bool isAdditive = false)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        LoadSceneMode loadSceneMode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneType, loadSceneMode);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (isAdditive == true)
        {
            Scene scene = SceneManager.GetSceneByName(sceneType);
            SceneManager.SetActiveScene(scene);
        }

        SceneBase sceneBase = GameObject.Find(sceneType).GetComponent<SceneBase>();
        sceneBase.OnInitalize();
    }

    private void OnChangedActiveScene(Scene prev, Scene next)
    {
        string prevName = prev.name;
        if (prevName.IsNullOrEmpty())
        {
            prevName = "Empty";
        }

        Debug.Log($"<color=green> [Change Scene] : CurrentActiveScene={prevName}, NextActiveScene={next.name}</color>");

        CurrentSceneType = next.name;
        SceneBase nextSceneBase = GameObject.Find(next.name).GetComponent<SceneBase>();
        nextSceneBase.OnStart();
    }

    public bool IsInGame()
    {
        return CurrentSceneType.Equals(EScene.INGAME);
    }

    public T GetSceneBase<T>() where T : SceneBase
    {
        T result = GameObject.Find(CurrentSceneType).GetComponent<SceneBase>() as T;
        return result;
    }
}
