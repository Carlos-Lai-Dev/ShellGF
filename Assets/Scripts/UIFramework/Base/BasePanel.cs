using UnityEngine;
public enum PanelName
{
    StartPanel,
    SettingsPanel,
    GamePanel,
    LoadingPanel,
}
public enum BtnName
{ 
    StartBtn,
    OptionsBtn,
    QuitBtn,
    BackBtn,
    CloseBtn,
}
public class BasePanel
{
    public UIType uiType { get; private set; }
    protected readonly static string resource = "Prefabs/UI/Panel/";
    public GameObject activeObject;
    protected BasePanel(UIType uIType)
    {
        uiType = uIType;
    }
    private void PanelIsEnable(bool isEnable)
    {
        activeObject.GetOrAddComponent<CanvasGroup>().interactable = isEnable;
    }
    public virtual void OnEnter() { }
    public virtual void OnEnable() 
    {
        PanelIsEnable(true);
    }
    public virtual void OnDisable() 
    {
        PanelIsEnable(false);
    }
    public virtual void OnExit() { }
}
