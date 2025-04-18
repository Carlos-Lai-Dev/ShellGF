using UnityEngine.SceneManagement;
namespace ShellGF.Runtime
{
    public class StartScene : SceneBase
    {
        public override void EnterScene()
        {
            base.EnterScene();
        }

        public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            UIManager.GetInstance().OpenPanel(new StartPanel());
            SoundManager.SetBGM(SoundName.StartBGM);
        }

        public override void ExitScene()
        {
            base.ExitScene();
        }

    }

}
