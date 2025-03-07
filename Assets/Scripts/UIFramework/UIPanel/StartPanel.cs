using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button startBtn;
    private Button optionsBtn;
    private Button quitBtn;
    private static readonly string path = resource + PanelName.StartPanel.ToString();
    public StartPanel() : base(new UIType(path))
    {
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        startBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.StartBtn);
        optionsBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.OptionsBtn);
        quitBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.QuitBtn);
        startBtn.AddButtonSounds();
        optionsBtn.AddButtonSounds();
        quitBtn.AddButtonSounds();
        startBtn.onClick.AddListener(LoadGame);
        optionsBtn.onClick.AddListener(OpenSettingsPanel);
        quitBtn.onClick.AddListener(QuitGame);
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
        Application.Quit();
#endif
    }
}
