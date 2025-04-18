using UnityEngine.UI;
namespace ShellGF.Runtime
{
    public class GamePanel : BasePanel
    {
        private Button backBtn;
        private static readonly string assetName = PanelName.GamePanel.ToString();

        public GamePanel() : base(new UIType(assetName))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            backBtn = activeObject.GetOrAddComponentInChildren<Button>(BtnName.BackBtn);
            backBtn.onClick.AddListenerWithSound(BackToStart);
        }

        private void BackToStart()
        {
            SceneController.GetInstance().LoadScene(SceneName.StartScene, new StartScene());
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnDisable()
        {
            base.OnDisable();
        }
        public override void OnExit()
        {
            base.OnExit();
            backBtn.onClick.RemoveAllListeners();
        }
    }
}

