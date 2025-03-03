using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private static GameRoot instance;
    //public UIManager UIManager_GR { get; private set; }
    //public SceneController SceneController_GR { get; private set; }
    public GameRoot GetInstance() => instance != null ? instance : null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        //UIManager_GR =
        new UIManager();
        //SceneController_GR =
        new SceneController();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        StartScene startScene = new StartScene();
        SceneController.GetInstance().scene_Dic[startScene.sceneName] = startScene;
        UIManager.GetInstance().PushPanel(new StartPanel());
    }

}
