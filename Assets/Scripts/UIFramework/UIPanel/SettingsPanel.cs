using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    private Button closeBtn;
    private static readonly string path = resource + PanelName.SettingsPanel.ToString();
    public SettingsPanel() : base(new UIType(path))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        closeBtn = UIMethod.GetInstance().GetOrAddComponentInChildren<Button>(activeTransform, BtnName.CloseBtn);
        closeBtn.onClick.AddListener(CloseSettingsPanel);
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
        closeBtn.onClick.RemoveListener(CloseSettingsPanel);
    }
    private void CloseSettingsPanel()
    {
        UIManager.GetInstance().ClosePanel();
    }
}
