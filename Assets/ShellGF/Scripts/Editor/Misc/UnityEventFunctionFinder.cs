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

        [MenuItem("Tools/UnityEvent/�¼����ò���")]
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

                // ��ʾ�ҵ��� GameObject �б�
                foreach (var go in gameObjectsWithFunction)
                {
                    if (go != null)
                    {
                        // ����һ����ť����λ GameObject
                        if (GUILayout.Button(go.name))
                        {
                            Selection.activeGameObject = go;  // ����ѡ�� GameObject
                            EditorGUIUtility.PingObject(go);  // �ڱ༭���и�����ʾ
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

            // ��ȡ���������е� GameObject
            var allGameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);



            foreach (var go in allGameObjects)
            {
                // ��ȡ GameObject �ϵ��������
                var components = go.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component == null)
                        continue;

                    // �������� UnityEvent �ֶ�
                    var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var field in fields)
                    {
                        if (field.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                        {
                            var unityEvent = field.GetValue(component) as UnityEventBase;
                            if (unityEvent != null)
                            {
                                // ��� UnityEvent ���Ƿ����Ŀ�꺯��
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

                    // �ر��� Button ����� OnClick �¼�
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
