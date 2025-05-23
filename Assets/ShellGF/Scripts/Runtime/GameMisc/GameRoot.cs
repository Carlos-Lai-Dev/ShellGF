using UnityEngine;

namespace ShellGF.Runtime
{
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
            gameObject.AddComponent<ABMemoryDisplay>();
            gameObject.AddComponent<ABMemoryTracker>();
            gameObject.AddComponent<ABReferenceManager>();
        }
        private void Start()
        {
            UIManager.GetInstance().OpenPanel(new StartPanel());
        }
    }
}


