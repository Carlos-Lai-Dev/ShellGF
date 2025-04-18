using System;
using UnityEngine;
using System.Collections.Generic;
namespace ShellGF.Runtime
{
    public class ABMemoryTracker : MonoBehaviour
    {
        public readonly static Dictionary<string, ABMemoryInfo> _loadedBundles = new Dictionary<string, ABMemoryInfo>();

        public class ABMemoryInfo
        {
            public string bundleName;
            public DateTime loadTime;
            public DateTime lastAccessTime;
            public int referenceCount;
            public List<string> assetNames = new List<string>();
        }

        // 注册已加载的AB包
        public static void RegisterBundleLoad(AssetBundle bundle, string bundleName)
        {
            if (!_loadedBundles.ContainsKey(bundleName))
            {
                var info = new ABMemoryInfo()
                {
                    bundleName = bundleName,
                    loadTime = DateTime.Now,
                    lastAccessTime = DateTime.Now,
                };

                _loadedBundles.Add(bundleName, info);
            }

            UpdateBundleAssets(bundle, bundleName);
        }
        private static void UpdateBundleAssets(AssetBundle bundle, string bundleName)
        {
            if (!_loadedBundles.TryGetValue(bundleName, out var info))
                return;

            // 清空现有资产列表（如果是重新加载）
            info.assetNames.Clear();

            // 获取AB包中的所有资产
            UnityEngine.Object[] assets = bundle.LoadAllAssets();

            // 记录每个资产信息
            foreach (var asset in assets)
            {
                if (asset != null)
                {
                    info.assetNames.Add($"{asset.name} ({asset.GetType().Name})");

                    // 如果是GameObject/Prefab，可能需要递归记录其组件
                    if (asset is GameObject go)
                    {
                        foreach (var comp in go.GetComponents<Component>())
                        {
                            if (comp != null)
                            {
                                info.assetNames.Add($"  |- {comp.GetType().Name}");
                            }
                        }
                    }
                }
            }

        }

    }
}
