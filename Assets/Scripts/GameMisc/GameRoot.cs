using UnityEngine;

public class GameRoot : SingletonMono<GameRoot>
{
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Destroy GameRoot !");
#endif
            Destroy(gameObject);
        }

    }
    private void Init()
    {
        SoundManager.Init();
    }
    private void Start()
    {
        UIManager.GetInstance().OpenPanel(new StartPanel());
    }
}
