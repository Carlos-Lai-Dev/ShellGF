using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    private Button closeBtn;
    private static readonly string assetName = PanelName.SettingsPanel.ToString();
    public SettingsPanel() : base(new UIType(assetName))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        closeBtn = activeObject.GetOrAddComponentInChildren<Button>(BtnName.CloseBtn);
        closeBtn.onClick.AddListenerWithSound(CloseSettingsPanel);
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
        closeBtn.onClick.RemoveAllListeners();
    }
    private void CloseSettingsPanel()
    {
        UIManager.GetInstance().ClosePanel();
    }
}
