using UnityEngine.SceneManagement;
namespace ShellGF.Runtime   
{
    public enum SceneName
    {
        StartScene,
        GameScene,
        LoadingScene,
    }
    public abstract class SceneBase
    {
        public virtual void EnterScene()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public abstract void OnSceneLoaded(Scene arg0, LoadSceneMode arg1);

        public virtual void ExitScene()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}


