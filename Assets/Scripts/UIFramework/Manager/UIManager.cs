using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Transform parent;
    private static UIManager instance;
    public static UIManager GetInstance() => instance != null ? instance : null;

    private Stack<BasePanel> ui_Stack;
    private Dictionary<UIType, GameObject> ui_Dic;
    public UIManager()
    {
        if (instance == null) instance = this;
        ui_Stack = new Stack<BasePanel>();
        ui_Dic = new Dictionary<UIType, GameObject>();
        FindCanvas();
    }

    private void FindCanvas()
    {
        UIMethod.GetInstance().FindCanvas(ref parent);
    }
    public GameObject GetSingleObject(UIType type)
    {
        if (ui_Dic.ContainsKey(type)) return ui_Dic[type];
        if (parent == null) FindCanvas();
        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent);
        ui.name = type.Name;
        ui_Dic[type] = ui;
        return ui;
    }
    public void PushPanel(BasePanel panel)
    {
#if UNITY_EDITOR
        Debug.Log($"{panel.uiType.Name} Push Stack !");
#endif
        if (ui_Stack.Count > 0) ui_Stack.Peek().OnDisable();

        panel.activeObject = GetSingleObject(panel.uiType);

        if (ui_Stack.Count == 0 || (ui_Stack.Count != 0 && panel.uiType.Name != ui_Stack.Peek().uiType.Name))
        {
            ui_Stack.Push(panel);
            panel.OnEnter();
        }
      
        panel.OnEnable();
    }
    public void PopPanel(bool isEmpty = false)
    {

        if (ui_Stack.Count > 0)
        {
            ui_Stack.Peek().OnDisable();
            ui_Stack.Peek().OnExit();
#if UNITY_EDITOR
            Debug.Log($"{ui_Stack.Peek().uiType.Name} Pop Stack !");
#endif
            GameObject.Destroy(ui_Dic[ui_Stack.Peek().uiType]);
            ui_Dic.Remove(ui_Stack.Peek().uiType);
            ui_Stack.Pop();

            if (isEmpty)
            {
                PopPanel(isEmpty);
            }
            else if (ui_Stack.Count > 0)
            {
                ui_Stack.Peek().OnEnable();
            }
        }

    }

}
