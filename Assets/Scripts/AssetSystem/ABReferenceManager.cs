using System;
using UnityEngine;
using System.Collections.Generic;

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

    public static void ReleaseReference(string bundleName)
    {
        if (_referenceCounts.ContainsKey(bundleName))
        {
            _referenceCounts[bundleName]--;

            // 更新内存监控信息
            if (ABMemoryTracker._loadedBundles.TryGetValue(bundleName, out var info))
            {
                info.referenceCount = _referenceCounts[bundleName];
                info.lastAccessTime = DateTime.Now;
            }

            // 引用计数为0时可以考虑卸载
            if (_referenceCounts[bundleName] <= 0)
            {
                _referenceCounts.Remove(bundleName);
                ABManager.GetInstance().UnLoad(bundleName);
                ABMemoryTracker._loadedBundles.Remove(bundleName);
            }
        }
    }
}
