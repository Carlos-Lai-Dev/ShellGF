using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : BasePanel
{
    private static readonly string path = "Prefabs/UI/Panel/StartPanel";
    public StartPanel() : base(new UIType(path))
    {

    }
}
