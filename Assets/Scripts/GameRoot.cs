using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private static GameRoot instance;
    //public UIManager UIManager_GR { get; private set; }
    //public SceneController SceneController_GR { get; private set; }
    public static GameRoot GetInstance() => instance ?? null;
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
        SoundManager.Init();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        UIManager.GetInstance().OpenPanel(new StartPanel());
    }
}
