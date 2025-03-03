public enum SceneName
{
    StartScene,
    GameScene,
}
public abstract class SceneBase
{
    public string sceneName;
    public abstract void EnterScene();
    public abstract void ExitScene();
}

