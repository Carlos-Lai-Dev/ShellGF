using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ABMemoryMonitorWindow : EditorWindow
{
    [MenuItem("Tools/AssetBundle Memory Monitor")]
    public static void ShowWindow()
    {
        GetWindow<ABMemoryMonitorWindow>("AB Memory Monitor");
    }

    private Vector2 _scrollPos;
    private bool _showDetails = false;

    void OnGUI()
    {
        GUILayout.Label("AssetBundle Memory Usage", EditorStyles.boldLabel);

        // 列表显示
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        foreach (var bundle in ABMemoryTracker._loadedBundles.Values)
        {
            EditorGUILayout.BeginHorizontal();

            // 显示基本信息
            EditorGUILayout.LabelField(bundle.bundleName, GUILayout.Width(200));
            EditorGUILayout.LabelField(bundle.referenceCount.ToString(), GUILayout.Width(50));

            // 显示/隐藏详情按钮
            if (GUILayout.Button(_showDetails ? "Hide" : "Show", GUILayout.Width(50)))
            {
                _showDetails = !_showDetails;
            }

            EditorGUILayout.EndHorizontal();

            // 显示详情
            if (_showDetails)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField($"Loaded: {bundle.loadTime}");
                EditorGUILayout.LabelField($"Last Access: {bundle.lastAccessTime}");
                EditorGUILayout.LabelField("Assets:");
                foreach (var asset in bundle.assetNames)
                {
                    EditorGUILayout.LabelField($"- {asset}");
                }
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndScrollView();
    }

}
#endif
