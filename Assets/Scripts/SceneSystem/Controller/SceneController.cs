using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController
{
    private static SceneController instance;
    private readonly Dictionary<string, SceneBase> scene_Dic;
    private static Action onLoaderCallback;
    private static AsyncOperation asyncOperation;
    public static SceneController GetInstance() => instance != null ? instance : instance = new SceneController();
    private class LoadingMono : MonoBehaviour { }
    private SceneController()
    {
        scene_Dic = new Dictionary<string, SceneBase>();
    }

    public void LoadScene(SceneName sceneName, SceneBase sceneBase)
    {
        string sceneNameStr = sceneName.ToString();
        if (!scene_Dic.ContainsKey(sceneNameStr)) scene_Dic[sceneNameStr] = sceneBase;

        if (scene_Dic.ContainsKey(SceneManager.GetActiveScene().name))
        {
            scene_Dic[SceneManager.GetActiveScene().name].ExitScene();
            scene_Dic.Remove(SceneManager.GetActiveScene().name);
        }
        UIManager.GetInstance().ClosePanel(true);
        onLoaderCallback = () =>
        {
            GameObject loadingObject = new GameObject("LoadingObject");
            loadingObject.AddComponent<LoadingMono>().StartCoroutine(LoadSceneAsync(sceneNameStr));
            sceneBase.EnterScene();
        };
        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
    }

    private static IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);
        asyncOperation.allowSceneActivation = true;
    }
    public static float GetLoadingProgress() => asyncOperation != null ? asyncOperation.progress : 0f;
    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
