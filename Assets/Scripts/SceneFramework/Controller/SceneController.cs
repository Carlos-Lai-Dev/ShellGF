using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneController
{
    private static SceneController instance;
    public Dictionary<string, SceneBase> scene_Dic;
    public static SceneController GetInstance() => instance != null ? instance : null;

    public SceneController()
    {
        if (instance == null) instance = this;
        scene_Dic = new Dictionary<string, SceneBase>();
    }
    public void LoadScene(string sceneName, SceneBase sceneBase)
    {
        if (!scene_Dic.ContainsKey(sceneName)) scene_Dic[sceneName] = sceneBase;

        if (scene_Dic.ContainsKey(SceneManager.GetActiveScene().name))
        {
            scene_Dic[SceneManager.GetActiveScene().name].ExitScene();
        }
        UIManager.GetInstance().PopPanel(true);
        SceneManager.LoadScene(sceneName);
        sceneBase.EnterScene();
    }
}
