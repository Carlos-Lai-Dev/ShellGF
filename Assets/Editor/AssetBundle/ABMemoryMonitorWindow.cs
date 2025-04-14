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

        // �б���ʾ
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        foreach (var bundle in ABMemoryTracker._loadedBundles.Values)
        {
            EditorGUILayout.BeginHorizontal();

            // ��ʾ������Ϣ
            EditorGUILayout.LabelField(bundle.bundleName, GUILayout.Width(200));
            EditorGUILayout.LabelField(bundle.referenceCount.ToString(), GUILayout.Width(50));

            // ��ʾ/�������鰴ť
            if (GUILayout.Button(_showDetails ? "Hide" : "Show", GUILayout.Width(50)))
            {
                _showDetails = !_showDetails;
            }

            EditorGUILayout.EndHorizontal();

            // ��ʾ����
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
