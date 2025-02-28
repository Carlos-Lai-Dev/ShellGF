using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    private Transform parent;
    private static UIManager instance;
    private bool hasInstance => instance != null;
    public UIManager GetInstance() => hasInstance ? instance : null;

    public Stack<BasePanel> ui_Stack;
    public Dictionary<UIType, GameObject> ui_Dic;
    public UIManager()
    {
        if (instance == null) instance = this;
        ui_Stack = new Stack<BasePanel>();
        ui_Dic = new Dictionary<UIType, GameObject>();
        FindCanvas();
    }
    private Transform CreateCanvas()
    {
        GameObject canvas = new GameObject("Canvas");
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<GraphicRaycaster>();
        var canvasScaler = canvas.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

#if UNITY_STANDALONE_WIN
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
#elif UNITY_ANDROID
        if (Screen.orientation == ScreenOrientation.Landscape)
            canvasScaler.referenceResolution = new Vector2(2400, 1080);
        else if (Screen.orientation == ScreenOrientation.Portrait)
            canvasScaler.referenceResolution = new Vector2(1080, 2400);
#endif

        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        return canvas.transform;
    }
    public void FindCanvas()
    {
        try
        {
            if (parent == null)
            {
                parent = GameObject.Find("Canvas").transform;
                if (parent == null) parent = CreateCanvas();
            }
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogException(e);
#endif
        }

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
        if (ui_Stack.Count == 0)
        {
            ui_Stack.Push(panel);
        }
        else
        {
            if (panel.uiType.Name != ui_Stack.Peek().uiType.Name)
            {
                ui_Stack.Push(panel);
            }
        }
        panel.OnEnter();
    }
    public void PopPanel(bool isEmpty)
    {

        if (ui_Stack.Count > 0)
        {
            ui_Stack.Peek().OnDisable();
            ui_Stack.Peek().OnExit();
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
