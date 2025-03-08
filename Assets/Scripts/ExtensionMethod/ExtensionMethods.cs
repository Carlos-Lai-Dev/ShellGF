using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ExtensionMethods
{
    /// <summary>
    /// 额外添加音频播放
    /// </summary>
    /// <param name="buttonClicked"></param>
    /// <param name="call"></param>
    public static void AddListenerWithSound(this Button.ButtonClickedEvent buttonClicked, UnityAction call)
    {
        buttonClicked.AddListener(() => { SoundManager.PlaySound(SoundName.BtnClick); });
        buttonClicked.AddListener(call);
    }
    /// <summary>
    /// 获取或添加组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if (!gameObject.TryGetComponent<T>(out T component))
            component = gameObject.AddComponent<T>();

        return component;
    }
    public static GameObject FindObjectInChildren(this GameObject parent, string childName)
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

    public static T GetOrAddComponentInChildren<T>(this GameObject  parent, BtnName childName) where T : Component
    {
        var go = FindObjectInChildren(parent, childName.ToString());
        if (go)
        {
            return go.GetOrAddComponent<T>();
        }
        return null;
    }
    public static T GetOrAddComponentInChildren<T>(this GameObject parent, string childName) where T : Component
    {
        var go = FindObjectInChildren(parent, childName);
        if (go)
        {
            return go.GetOrAddComponent<T>();
        }
        return null;
    }
    /// <summary>
    /// 检查GameObject是否在指定Layer中
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << gameObject.layer));
    }


    /// <summary>
    /// 重置Transform的位置、旋转和缩放
    /// </summary>
    /// <param name="transform"></param>
    public static void Reset(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置X轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    /// <summary>
    /// 设置Y轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    /// <summary>
    /// 设置Z轴位置
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="z"></param>
    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
    /// <summary>
    /// 将Vector3的Y轴设置为0
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// 计算两个Vector3之间的水平距离
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static float HorizontalDistance(this Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from.Flatten(), to.Flatten());
    }
    /// <summary>
    /// 随机打乱List中的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 获取List中的随机元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <exception cref="System.IndexOutOfRangeException"></exception>
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("List is empty");
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// 设置颜色的透明度
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// 设置RectTransform的锚点位置
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="anchor"></param>
    public static void SetAnchor(this RectTransform rectTransform, Vector2 anchor)
    {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
    }

    /// <summary>
    /// 设置RectTransform的宽度
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="width"></param>
    public static void SetWidth(this RectTransform rectTransform, float width)
    {
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }

    /// <summary>
    /// 设置RectTransform的高度
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="height"></param>
    public static void SetHeight(this RectTransform rectTransform, float height)
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
    }

    /// <summary>
    /// 检查LayerMask是否包含指定层
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    /// <summary>
    /// 将Quaternion限制在某个角度范围内
    /// </summary>
    /// <param name="q"></param>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    /// <returns></returns>
    public static Quaternion Clamp(this Quaternion q, float minAngle, float maxAngle)
    {
        q.ToAngleAxis(out float angle, out Vector3 axis);
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        return Quaternion.AngleAxis(angle, axis);
    }

    /// <summary>
    /// 检查某个点是否在相机视野内
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="worldPoint"></param>
    /// <returns></returns>
    public static bool IsVisible(this Camera camera, Vector3 worldPoint)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(worldPoint);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
    }
}
