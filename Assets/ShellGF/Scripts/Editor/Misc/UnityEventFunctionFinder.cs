using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShellGF.Editor
{
    public class UnityEventFunctionFinder : EditorWindow
    {
        private string functionName = "";
        private List<GameObject> gameObjectsWithFunction = new List<GameObject>();

        [MenuItem("Tools/UnityEvent/事件调用查找")]
        public static void ShowWindow()
        {
            GetWindow<UnityEventFunctionFinder>("UnityEvent Function Finder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find GameObjects Using Function in UnityEvent", EditorStyles.boldLabel);

            functionName = EditorGUILayout.TextField("Function Name", functionName);

            if (GUILayout.Button("Find Functions"))
            {
                FindFunctionsInUnityEvents(functionName);
            }

            if (gameObjectsWithFunction.Count > 0)
            {
                GUILayout.Space(10);
                GUILayout.Label("GameObjects with the function:", EditorStyles.boldLabel);

                // 显示找到的 GameObject 列表
                foreach (var go in gameObjectsWithFunction)
                {
                    if (go != null)
                    {
                        // 创建一个按钮来定位 GameObject
                        if (GUILayout.Button(go.name))
                        {
                            Selection.activeGameObject = go;  // 快速选择 GameObject
                            EditorGUIUtility.PingObject(go);  // 在编辑器中高亮显示
                        }
                    }
                }
            }
            else
            {
                GUILayout.Label("No GameObjects found with the specified function.", EditorStyles.boldLabel);
            }
        }

        private void FindFunctionsInUnityEvents(string funcName)
        {
            gameObjectsWithFunction.Clear();

            // 获取场景中所有的 GameObject
            var allGameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);



            foreach (var go in allGameObjects)
            {
                // 获取 GameObject 上的所有组件
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == null)
                        continue;

                    // 查找所有 UnityEvent 字段
                    var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        if (field.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                        {
                            var unityEvent = field.GetValue(component) as UnityEventBase;
                            if (unityEvent != null)
                            {
                                // 检查 UnityEvent 中是否包含目标函数
                                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                                {
                                    var target = unityEvent.GetPersistentTarget(i);
                                    var methodName = unityEvent.GetPersistentMethodName(i);

                                    if (methodName == funcName)
                                    {
                                        gameObjectsWithFunction.Add(go);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // 特别处理 Button 组件的 OnClick 事件
                    if (component is Button button)
                    {
                        var buttonOnClick = button.onClick;
                        for (int i = 0; i < buttonOnClick.GetPersistentEventCount(); i++)
                        {
                            var target = buttonOnClick.GetPersistentTarget(i);
                            var methodName = buttonOnClick.GetPersistentMethodName(i);

                            if (methodName == funcName)
                            {
                                gameObjectsWithFunction.Add(go);
                                break;
                            }
                        }
                    }
                }
            }

            if (gameObjectsWithFunction.Count == 0)
            {
                Debug.Log($"No UnityEvent found with method '{funcName}'.");
            }
            else
            {
                Debug.Log($"Found {gameObjectsWithFunction.Count} GameObject(s) with the method '{funcName}'.");
            }
        }
    }
}
