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
#if UNITY_EDITOR
        gameObject.AddComponent<ABMemoryDisplay>();
        gameObject.AddComponent<ABMemoryTracker>();
#endif
        gameObject.AddComponent<ABMemoryWatcher>();
        gameObject.AddComponent<ABReferenceManager>();
    }
    private void Start()
    {
        UIManager.GetInstance().OpenPanel(new StartPanel());
    }
}
