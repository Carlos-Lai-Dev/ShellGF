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
        {

            instance = this;
            new UIManager();
            new SceneController();
            SoundManager.Init();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Destroy GameRoot !");
#endif
            Destroy(gameObject);
        }
        //UIManager_GR =
        //SceneController_GR =
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        UIManager.GetInstance().OpenPanel(new StartPanel());
    }
}
