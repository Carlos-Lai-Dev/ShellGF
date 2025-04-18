using System;
using System.Collections.Generic;
using UnityEngine;
namespace ShellGF.Runtime
{
    public class ABReferenceManager : MonoBehaviour
    {
        private readonly static Dictionary<string, int> _referenceCounts = new Dictionary<string, int>();

        public static void AddReference(string bundleName)
        {

            if (_referenceCounts.ContainsKey(bundleName))
            {
                _referenceCounts[bundleName]++;
            }
            else
            {
                _referenceCounts[bundleName] = 1;
            }

            // 更新内存监控信息
            if (ABMemoryTracker._loadedBundles.TryGetValue(bundleName, out var info))
            {
                info.referenceCount = _referenceCounts[bundleName];
                info.lastAccessTime = DateTime.Now;
            }
        }

        private static void ReleaseReference(string bundleName, bool unLoadAllLoadedObjects)
        {
            if (_referenceCounts.ContainsKey(bundleName))
            {
                Queue<string> load_Queue = new Queue<string>();
                load_Queue.Enqueue(bundleName);

                for (; load_Queue.Count > 0; load_Queue.Dequeue())
                {
                    string name = load_Queue.Peek();
                    _referenceCounts[name]--;

                    foreach (var depend in ABManager.manifest.GetAllDependencies(name))
                    {
                        load_Queue.Enqueue(depend);
                    }
                    // 更新内存监控信息
                    if (ABMemoryTracker._loadedBundles.TryGetValue(name, out var info))
                    {
                        info.referenceCount = _referenceCounts[name];
                        info.lastAccessTime = DateTime.Now;
                    }

                    // 引用计数为0时可以考虑卸载
                    if (_referenceCounts[name] <= 0)
                    {
                        RemoveReference(name);
                        ABManager.GetInstance().UnLoad(name, unLoadAllLoadedObjects);
                        ABMemoryTracker._loadedBundles.Remove(name);
                    }
                }



            }
        }
        public static void ReleaseReference(ABName bundleName, bool unLoadAllLoadedObjects)
        {
            ReleaseReference(bundleName.ToString(), unLoadAllLoadedObjects);
        }
        public static void RemoveReference(string bundleName)
        {
            _referenceCounts.Remove(bundleName);
        }
    }
}

