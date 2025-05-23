using UnityEngine; //宏引用不能取消
using UnityEngine.UI;

namespace ShellGF.Runtime
{
    public class StartPanel : BasePanel
    {
        private Button startBtn;
        private Button optionsBtn;
        private Button quitBtn;
        private static readonly string assetName = PanelName.StartPanel.ToString();
        public StartPanel() : base(new UIType(assetName))
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            startBtn = activeObject.GetOrAddComponentInChildren<Button>(BtnName.StartBtn);
            optionsBtn = activeObject.GetOrAddComponentInChildren<Button>(BtnName.OptionsBtn);
            quitBtn = activeObject.GetOrAddComponentInChildren<Button>(BtnName.QuitBtn);
            startBtn.onClick.AddListenerWithSound(LoadGame);
            optionsBtn.onClick.AddListenerWithSound(OpenSettingsPanel);
            quitBtn.onClick.AddListenerWithSound(QuitGame);
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
            startBtn.onClick.RemoveAllListeners();
            optionsBtn.onClick.RemoveAllListeners();
            quitBtn.onClick.RemoveAllListeners();
        }
        private void LoadGame()
        {
            SceneController.GetInstance().LoadScene(SceneName.GameScene, new GameScene());
        }
        private void OpenSettingsPanel()
        {
            UIManager.GetInstance().OpenPanel(new SettingsPanel());
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        ABManager.GetInstance().UnLoadAllAssetBundles(true);
        Application.Quit();
#endif
        }
    }
}

