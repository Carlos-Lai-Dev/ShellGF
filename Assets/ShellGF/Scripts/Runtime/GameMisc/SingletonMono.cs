using UnityEngine;

namespace ShellGF.Runtime
{
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        public static T GetInstance() => Instance;
        private static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var obj = new GameObject(typeof(T).Name);
                        DontDestroyOnLoad(obj);
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }

        }

    }
}

