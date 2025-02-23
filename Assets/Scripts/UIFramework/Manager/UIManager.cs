using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager
{
    public static Transform parent;

    public Dictionary<UIType, GameObject> UI_Dic;
    public UIManager()
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

        if (parent == null)
        {
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            var canasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            canasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
           
#if     UNITY_STANDALONE_WIN
            canasScaler.referenceResolution = new Vector2(1920f, 1080f);
#elif   UNITY_ANDROID
            if(Screen.orientation == ScreenOrientation.Portrait)
                canasScaler.referenceResolution = new Vector2(1080f, 2400f);
            else if(Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
                canasScaler.referenceResolution = new Vector2(2400f, 1080f);
#endif
            canasScaler.matchWidthOrHeight = 0.5f;
            EventSystem eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();

            parent = canvas.gameObject.transform;
           
        }

        UI_Dic = new Dictionary<UIType, GameObject>();
    }

    public GameObject GetUI(UIType type)
    {
        if (parent == null)
        {
            parent = GameObject.Find("Canvas").transform;
            if (parent == null) return null;
        }

        if (UI_Dic.ContainsKey(type)) return UI_Dic[type];

        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent);
        ui.name = type.Name;
        UI_Dic[type] = ui;
        return ui;
    }

    public void DestroyUI(UIType type)
    {
        if (UI_Dic.ContainsKey(type))
        {
            GameObject.Destroy(UI_Dic[type]);
            UI_Dic.Remove(type);
        }
    }
}
