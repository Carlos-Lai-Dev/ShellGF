using UnityEngine;
public enum PanelName
{
    StartPanel,
    SettingPanel,
}
public enum BtnName
{ 
    StartBtn,
    SettingBtn,
}
public class BasePanel
{
    public UIType uiType { get; private set; }
    protected readonly static string resource = "Prefabs/UI/Panel/";
    public GameObject activeObject;
    public Transform activeTransform => activeObject.transform;
    protected BasePanel(UIType uIType)
    {
        uiType = uIType;
    }
    private void PanelIsEnable(bool isEnable)
    {
        UIMethod.GetInstance().GetOrAddComponent<CanvasGroup>(activeTransform).interactable = isEnable;
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
