using System;
using UnityEngine;
using System.Collections.Generic;

public class ABMemoryTracker : MonoBehaviour
{
    public readonly static Dictionary<string, ABMemoryInfo> _loadedBundles = new Dictionary<string, ABMemoryInfo>();

    public class ABMemoryInfo
    {
        public string bundleName;
        public long memorySize; // �ڴ�ռ���ֽ���
        public DateTime loadTime;
        public DateTime lastAccessTime;
        public int referenceCount;
        public List<string> assetNames = new List<string>();
    }

    // ע���Ѽ��ص�AB��
    public static void RegisterBundleLoad(AssetBundle bundle, string bundleName)
    {
        if (!_loadedBundles.ContainsKey(bundleName))
        {
            var info = new ABMemoryInfo()
            {
                bundleName = bundleName,
                loadTime = DateTime.Now,
                lastAccessTime = DateTime.Now,
                memorySize = CalculateBundleSize(bundle)
            };

            _loadedBundles.Add(bundleName, info);
        }

        UpdateBundleAssets(bundle, bundleName);
    }
    private static void UpdateBundleAssets(AssetBundle bundle, string bundleName)
    {
        if (!_loadedBundles.TryGetValue(bundleName, out var info))
            return;

        // ��������ʲ��б���������¼��أ�
        info.assetNames.Clear();

        // ��ȡAB���е������ʲ�
        UnityEngine.Object[] assets = bundle.LoadAllAssets();

        // ��¼ÿ���ʲ���Ϣ
        foreach (var asset in assets)
        {
            if (asset != null)
            {
                info.assetNames.Add($"{asset.name} ({asset.GetType().Name})");

                // �����GameObject/Prefab��������Ҫ�ݹ��¼�����
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

        // ���¼����ڴ��С����Ϊ�ʲ������ѱ仯��
        info.memorySize = CalculateBundleSize(bundle);
    }
    private static long CalculateBundleSize(AssetBundle bundle)
    {
        // ����AB���ڴ�ռ��
        long size = 0;
        foreach (var asset in bundle.LoadAllAssets<UnityEngine.Object>())
        {
            size += EstimateMemoryUsage(asset);
        }
        return size;
    }

    private static long EstimateMemoryUsage(UnityEngine.Object obj)
    {
        // �򵥹�������ڴ�ռ��
        if (obj is Texture2D texture)
            return texture.width * texture.height * 4; // ����RGBA32��ʽ
        if (obj is Mesh mesh)
            return mesh.vertexCount * 12 + mesh.triangles.Length * 4;
        // �������͹���...
        return 1024; // Ĭ��1KB
    }
}