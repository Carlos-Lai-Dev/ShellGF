using UnityEngine;
using System.Collections.Generic;

namespace ShellGF.Runtime
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField] PoolArr[] poolArr;
        static Dictionary<GameObject, Pool> prefabs_Dic;

        private void Awake()
        {
            prefabs_Dic = new Dictionary<GameObject, Pool>();
            InitPool_Arr(poolArr);
        }
#if UNITY_EDITOR
        private void OnDestroy()
        {
            CheckPool(poolArr);
        }

        private void CheckPool(PoolArr[] pools)
        {
            foreach (var pool in pools)
            {
                foreach (var item in pool.pools)
                {
                    if (item.PoolSize < item.RuntimeSize)
                    {
                        Debug.LogWarning($"Pool:{item.Prefab.name} Runtime Size {item.RuntimeSize}is bigger than Init Size {item.PoolSize}");
                    }
                }

            }
        }
#endif


        void InitPool_Arr(PoolArr[] poolArr)
        {
            foreach (var pool in poolArr)
            {
                foreach (var item in pool.pools)
                {
#if UNITY_EDITOR
                    if (prefabs_Dic.ContainsKey(item.Prefab))
                    {
                        Debug.LogError("Has Same key in the Dictionary!");
                        continue;
                    }
#endif

                    prefabs_Dic.Add(item.Prefab, item);
                    var obj = new GameObject($"Pool:{item.Prefab.name}");

                    obj.transform.parent = transform;
                    item.InitPool(obj.transform);
                }

            }
        }

        public static GameObject Release(GameObject prefab)
        {
#if UNITY_EDITOR
            if (!prefabs_Dic.ContainsKey(prefab))
            {
                Debug.LogError("The key Can not found!");
                return null;
            }
#endif

            return prefabs_Dic[prefab].EnableObject();
        }

        public static GameObject Release(GameObject prefab, Vector3 setPos)
        {
#if UNITY_EDITOR
            if (!prefabs_Dic.ContainsKey(prefab))
            {
                Debug.LogError($"{prefab} The key Can not found!");
                return null;
            }
#endif

            return prefabs_Dic[prefab].EnableObject(setPos);
        }

        public static GameObject Release(GameObject prefab, Vector3 setPos, Quaternion setRots)
        {
#if UNITY_EDITOR
            if (!prefabs_Dic.ContainsKey(prefab))
            {
                Debug.LogError("The key Can not found!");
                return null;
            }
#endif

            return prefabs_Dic[prefab].EnableObject(setPos, setRots);
        }

        public static GameObject Release(GameObject prefab, Vector3 setPos, Quaternion setRots, Vector3 setScale)
        {
#if UNITY_EDITOR
            if (!prefabs_Dic.ContainsKey(prefab))
            {
                Debug.LogError("The key Can not found!");
                return null;
            }
#endif

            return prefabs_Dic[prefab].EnableObject(setPos, setRots, setScale);
        }

    }
}

