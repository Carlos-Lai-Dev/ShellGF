using UnityEditor;
using UnityEngine;

public class MultiComponentCopier : EditorWindow
{
    private GameObject sourceObject;
    private bool[] componentSelection;
    private Vector2 scrollPos;

    [MenuItem("Tools/Multi-Component Copier")]
    public static void ShowWindow()
    {
        GetWindow<MultiComponentCopier>("Component Copier");
    }

    private void OnGUI()
    {
        GUILayout.Label("Multi-Component Copy Tool", EditorStyles.boldLabel);

        sourceObject = EditorGUILayout.ObjectField("Source GameObject", sourceObject, typeof(GameObject), true) as GameObject;

        if (sourceObject == null)
        {
            EditorGUILayout.HelpBox("Please select a source GameObject", MessageType.Info);
            return;
        }

        var components = sourceObject.GetComponents<Component>();
        if (componentSelection == null || componentSelection.Length != components.Length)
        {
            componentSelection = new bool[components.Length];
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < components.Length; i++)
        {
            // Skip Transform component as it's always present
            if (components[i] is Transform) continue;

            componentSelection[i] = EditorGUILayout.ToggleLeft(components[i].GetType().Name, componentSelection[i]);
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Copy Selected Components"))
        {
            if (Selection.gameObjects.Length == 0)
            {
                Debug.LogError("No target GameObjects selected");
                return;
            }

            foreach (GameObject target in Selection.gameObjects)
            {
                if (target == sourceObject) continue;

                for (int i = 0; i < components.Length; i++)
                {
                    if (componentSelection[i])
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(components[i]);
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
                    }
                }
            }

            Debug.Log($"Copied selected components to {Selection.gameObjects.Length} target(s)");
        }
    }
}