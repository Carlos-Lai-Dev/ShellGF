using UnityEngine.UI;

public class GamePanel : BasePanel
{
    private Button backBtn;
    private static readonly string path = resource + PanelName.GamePanel.ToString();
    public GamePanel() : base(new UIType(path))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        backBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.BackBtn);
        backBtn.onClick.AddListener(BackToStart);
        backBtn.AddButtonSounds();
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
