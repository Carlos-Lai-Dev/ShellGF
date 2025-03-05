using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMethod
{
    private static UIMethod instance;
    public static UIMethod GetInstance() => instance == null ? instance = new UIMethod() : instance;
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

    public void FindCanvas(ref Transform parent)
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
    public T GetOrAddComponent<T>(Transform parent) where T : Component
    {
        if (parent.GetComponent<T>() == null) parent.gameObject.AddComponent<T>();
        return parent.GetComponent<T>(); 
    }

    public GameObject FindObjectInChildren(Transform parent, string childName)
    {
        Transform[] trans_Arr = parent.GetComponentsInChildren<Transform>();

        foreach (Transform trans in trans_Arr)
        {
            if (trans.name == childName) return trans.gameObject;
        }
#if UNITY_EDITOR
        Debug.Log($"In {parent} : The {childName} GameObject can't be Find !");
#endif
        return null;
    }

    public T GetOrAddComponentInChildren<T>(Transform parent, BtnName childName) where T : Component
    {
        var go = FindObjectInChildren(parent, childName.ToString());
        if (go)
        {
            if (go.GetComponent<T>() == null) go.AddComponent<T>();
            return go.GetComponent<T>();
        }
        return null;
    }
    public T GetOrAddComponentInChildren<T>(Transform parent, string childName) where T : Component
    {
        var go = FindObjectInChildren(parent, childName);
        if (go)
        {
            if (go.GetComponent<T>() == null) go.AddComponent<T>();
            return go.GetComponent<T>();
        }
        return null;
    }
}
