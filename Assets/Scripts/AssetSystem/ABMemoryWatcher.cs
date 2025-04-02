using System.Linq;
using UnityEngine;

public class ABMemoryWatcher : MonoBehaviour
{
    [Header("Memory Thresholds")]
    public long warningThreshold = 100 * 1024 * 1024; // 100MB
    public long criticalThreshold = 200 * 1024 * 1024; // 200MB

    [Header("Auto Unload Settings")]
    public bool autoUnload = true;
    public float checkInterval = 10f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= checkInterval)
        {
            _timer = 0;
            CheckMemory();
        }
    }

    private void CheckMemory()
    {
        long totalMemory = ABMemoryTracker._loadedBundles.Values.Sum(b => b.memorySize);

        if (totalMemory >= criticalThreshold)
        {
            Debug.LogError($"Critical AB Memory: {ExtensionMethods.FormatBytes(totalMemory)}");
            UnloadUnusedBundles(true);
        }
        else if (totalMemory >= warningThreshold)
        {
            Debug.LogWarning($"High AB Memory: {ExtensionMethods.FormatBytes(totalMemory)}");
            UnloadUnusedBundles(false);
        }
    }

    private void UnloadUnusedBundles(bool aggressive)
    {
        var bundlesToUnload = ABMemoryTracker._loadedBundles.Values
            .Where(b => b.referenceCount == 0)
            .OrderBy(b => b.lastAccessTime);

        if (aggressive)
        {
            // 激进模式：卸载所有无引用的AB包
            foreach (var bundle in bundlesToUnload)
            {
                ABManager.GetInstance().UnLoad(bundle.bundleName);
                ABMemoryTracker._loadedBundles.Remove(bundle.bundleName);
            }
        }
        else
        {
            // 保守模式：只卸载最旧的几个AB包
            foreach (var bundle in bundlesToUnload.Take(3))
            {
                ABManager.GetInstance().UnLoad(bundle.bundleName);
                ABMemoryTracker._loadedBundles.Remove(bundle.bundleName);
            }
        }
    }
}
