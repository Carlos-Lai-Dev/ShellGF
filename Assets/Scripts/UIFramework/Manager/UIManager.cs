using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    private Transform parent;
    private static UIManager instance;
    public static UIManager GetInstance() => instance = instance != null ? instance : new UIManager();

    private readonly Stack<BasePanel> ui_Stack;
    private readonly Dictionary<UIType, GameObject> ui_Dic;
    private UIManager()
    {
        ui_Stack = new Stack<BasePanel>();
        ui_Dic = new Dictionary<UIType, GameObject>();
        FindCanvas();
    }
    private Transform CreateCanvas()
    {
        GameObject go = new GameObject("Canvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<GraphicRaycaster>();
        var canvasScaler = go.AddComponent<CanvasScaler>();
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
        return go.transform;
    }

    private void FindCanvas()
    {
        try
        {
            if (parent == null) parent = GameObject.Find("Canvas").transform;
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning(e);
#endif
        }

        if (parent == null) parent = CreateCanvas();
    }

    private GameObject GetSingleObject(UIType type)
    {
        if (ui_Dic.ContainsKey(type)) return ui_Dic[type];
        if (parent == null) FindCanvas();
        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent);
        ui.name = type.Name;
        ui_Dic[type] = ui;
        return ui;
    }
    public void OpenPanel(BasePanel panel)
    {
#if UNITY_EDITOR
        Debug.Log($"{panel.uiType.Name} Open !");
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
    public void ClosePanel(bool isEmpty = false)
    {

        if (ui_Stack.Count > 0)
        {
            ui_Stack.Peek().OnDisable();
            ui_Stack.Peek().OnExit();
#if UNITY_EDITOR
            Debug.Log($"{ui_Stack.Peek().uiType.Name} Close !");
#endif
            GameObject.Destroy(ui_Dic[ui_Stack.Peek().uiType]);
            ui_Dic.Remove(ui_Stack.Peek().uiType);
            ui_Stack.Pop();

            if (isEmpty)
            {
                ClosePanel(isEmpty);
            }
            else if (ui_Stack.Count > 0)
            {
                ui_Stack.Peek().OnEnable();
            }
        }

    }

}
