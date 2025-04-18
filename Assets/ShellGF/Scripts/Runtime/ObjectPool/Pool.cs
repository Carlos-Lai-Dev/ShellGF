using UnityEngine;
using System.Collections.Generic;
using System;

namespace ShellGF.Runtime
{
    [Serializable]
    public class PoolArr
    {
        public Pool[] pools;
    }

    [Serializable]
    public class Pool
    {
        public int PoolSize => poolSize;
        public int RuntimeSize => queue.Count;

        [SerializeField] int poolSize;
        [SerializeField] GameObject prefab;
        Queue<GameObject> queue;
        Transform parentTrans;
        public GameObject Prefab => prefab;
        public void InitPool(Transform trans)
        {
            parentTrans = trans;
            queue = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                queue.Enqueue(Copy());
            }
        }

        GameObject Copy()
        {

            var copyObj = GameObject.Instantiate(prefab, parentTrans);
            copyObj.SetActive(false);

            return copyObj;

        }

        GameObject PickObject()
        {
            GameObject pickObj = null;
            if (queue.Count > 0 && !queue.Peek().activeSelf)
            {
                pickObj = queue.Dequeue();
            }
            else
            {
                pickObj = Copy();
            }
            queue.Enqueue(pickObj);
            return pickObj;
        }

        public GameObject EnableObject()
        {
            GameObject enableObj = PickObject();
            enableObj.SetActive(true);
            return enableObj;
        }

        public GameObject EnableObject(Vector3 setPos)
        {
            GameObject enableObj = PickObject();

            enableObj.transform.position = setPos;
            enableObj.SetActive(true);
            return enableObj;
        }

        public GameObject EnableObject(Vector3 setPos, Quaternion setRot)
        {
            GameObject enableObj = PickObject();

            enableObj.transform.position = setPos;
            enableObj.transform.rotation = setRot;
            enableObj.SetActive(true);
            return enableObj;
        }

        public GameObject EnableObject(Vector3 setPos, Quaternion setRot, Vector3 setScale)
        {
            GameObject enableObj = PickObject();

            enableObj.transform.position = setPos;
            enableObj.transform.rotation = setRot;
            enableObj.transform.localScale = setScale;
            enableObj.SetActive(true);
            return enableObj;
        }
    }

}
