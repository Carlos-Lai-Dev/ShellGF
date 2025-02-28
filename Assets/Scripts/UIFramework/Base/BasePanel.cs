using UnityEngine;

public class BasePanel
{
    public UIType uiType { get; private set; }
    public GameObject activeObject;

    protected BasePanel(UIType uIType)
    {
        uiType = uIType;
    }

    public virtual void OnEnter() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void OnExit() { }
}
