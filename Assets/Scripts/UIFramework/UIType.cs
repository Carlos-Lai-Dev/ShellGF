using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 存储单个UI的名字、路径
/// </summary>
public class UIType
{
    public string Name { get; private set; }
    public string Path { get; private set; }
    public UIType(string path)
    {
        Path = path;
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }
}
