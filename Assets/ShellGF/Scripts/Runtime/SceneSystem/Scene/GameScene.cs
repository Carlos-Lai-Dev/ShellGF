using UnityEngine.SceneManagement;
namespace ShellGF.Runtime
{
    public class GameScene : SceneBase
    {
        public override void EnterScene()
        {
            base.EnterScene();
        }
        public override void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            UIManager.GetInstance().OpenPanel(new GamePanel());
            SoundManager.SetBGM(SoundName.GameBGM);
        }
        public override void ExitScene()
        {
            base.ExitScene();
        }
    }

}
