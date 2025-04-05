using System;
using System.Collections.Generic;
using UnityEngine;

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

        // �����ڴ�����Ϣ
        if (ABMemoryTracker._loadedBundles.TryGetValue(bundleName, out var info))
        {
            info.referenceCount = _referenceCounts[bundleName];
            info.lastAccessTime = DateTime.Now;
        }
    }

    public static void ReleaseReference(string bundleName, bool unLoadAllLoadedObjects = false)
    {
        if (_referenceCounts.ContainsKey(bundleName))
        {
            _referenceCounts[bundleName]--;

            // �����ڴ�����Ϣ
            if (ABMemoryTracker._loadedBundles.TryGetValue(bundleName, out var info))
            {
                info.referenceCount = _referenceCounts[bundleName];
                info.lastAccessTime = DateTime.Now;
            }

            // ���ü���Ϊ0ʱ���Կ���ж��
            if (_referenceCounts[bundleName] <= 0)
            {
                RemoveReference(bundleName);
                ABManager.GetInstance().UnLoad(bundleName, unLoadAllLoadedObjects);
                ABMemoryTracker._loadedBundles.Remove(bundleName);
            }
        }
    }

    public static void RemoveReference(string bundleName)
    {
        _referenceCounts.Remove(bundleName);
    }
}
