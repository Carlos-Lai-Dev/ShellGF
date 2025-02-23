using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 所有UI的基类
/// </summary>
public abstract class BasePanel
{
    public UIType UIType { get; private set; }

    public BasePanel(UIType uIType)
    {
        UIType = uIType;
    }
    public virtual void OnEnter() { }
    public virtual void OnPause() { }
    public virtual void OnResume() { }
    public virtual void OnExit() { }
}
