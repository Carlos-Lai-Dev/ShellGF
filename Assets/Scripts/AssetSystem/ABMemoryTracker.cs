using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ABMemoryTracker : MonoBehaviour
{
    public readonly static Dictionary<string, ABMemoryInfo> _loadedBundles = new Dictionary<string, ABMemoryInfo>();

    public class ABMemoryInfo
    {
        public string bundleName;
        public long memorySize; // 内存占用字节数
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

        // 重新计算内存大小（因为资产可能已变化）
        info.memorySize = CalculateBundleSize(bundle);
    }
    private static long CalculateBundleSize(AssetBundle bundle)
    {
        // 估算AB包内存占用
        long size = 0;
        foreach (var asset in bundle.LoadAllAssets<UnityEngine.Object>())
        {
            size += EstimateMemoryUsage(asset);
        }
        return size;
    }
    private static Dictionary<Type, Func<UnityEngine.Object, long>> _sizeEstimators = new Dictionary<Type, Func<UnityEngine.Object, long>>
    {
        { typeof(Texture2D), EstimateTextureSize },
        { typeof(Mesh), EstimateMeshSize },
        { typeof(AudioClip), EstimateAudioSize },
        { typeof(Material), EstimateMaterialSize }
    };

    private static long EstimateMaterialSize(UnityEngine.Object obj)
    {
        if (obj is Material material)
        {
            long size = 1024; // 基础大小

            // 根据shader和纹理增加估算
            if (material.shader != null)
            {
                size += material.shader.passCount * 512;
            }

            // 纹理引用
            int textureCount = 0;
            for (int i = 0; i < ShaderUtil.GetPropertyCount(material.shader); i++)
            {
                if (ShaderUtil.GetPropertyType(material.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    textureCount++;
            }
            size += textureCount * 1024;

            return size;
        }
        return 0;
    }

    private static long EstimateAudioSize(UnityEngine.Object obj)
    {
        if (obj is AudioClip audioClip)
        {
            // 估算公式：采样数 × 通道数 × 位深/8
            int channels = audioClip.channels;
            int bitDepth = 16; // 假设16位
            float length = audioClip.length;
            int sampleRate = audioClip.frequency;

            return (long)(length * sampleRate * channels * (bitDepth / 8f));
        }
        return 0;
    }

    private static long EstimateMeshSize(UnityEngine.Object obj)
    {
        if (obj is Mesh mesh)
        {
            // 顶点数据
            long size = mesh.vertexCount * 12; // 假设每个顶点12字节(x,y,z × 4字节float)

            // 索引数据
            if (mesh.indexFormat == UnityEngine.Rendering.IndexFormat.UInt32)
                size += mesh.triangles.Length * 4;
            else
                size += mesh.triangles.Length * 2;

            // 其他属性(法线、UV等)
            if (mesh.normals != null && mesh.normals.Length > 0)
                size += mesh.vertexCount * 12;

            if (mesh.uv != null && mesh.uv.Length > 0)
                size += mesh.vertexCount * 8;

            // 蒙皮网格额外计算
            if (mesh.boneWeights != null && mesh.boneWeights.Length > 0)
                size += mesh.vertexCount * 16;

            return size;
        }
        return 0;
    }

    private static long EstimateTextureSize(UnityEngine.Object obj)
    {
        if (obj is Texture2D texture)
        {
            // 基础计算：宽 × 高 × 每像素字节数
            int bytesPerPixel = 4; // 默认按RGBA32格式

            // 根据实际格式调整
            switch (texture.format)
            {
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.BGRA32:
                    bytesPerPixel = 4; break;

                case TextureFormat.RGB24:
                    bytesPerPixel = 3; break;

                case TextureFormat.RGBAFloat:
                case TextureFormat.RGBAHalf:
                    bytesPerPixel = 16; break;

                case TextureFormat.BC7:
                case TextureFormat.DXT5:
                    // 压缩纹理按1字节/像素估算
                    bytesPerPixel = 1; break;

                default:
                    bytesPerPixel = 4; break;
            }

            // 包含mipmaps
            float mipmapFactor = texture.mipmapCount > 1 ? 1.33f : 1f;

            return (long)(texture.width * texture.height * bytesPerPixel * mipmapFactor);
        }
        return 0;
    }

    private static long EstimateMemoryUsage(UnityEngine.Object obj)
    {
        // 简单估算对象内存占用
        if (obj == null) return 0;

        var type = obj.GetType();
        if (_sizeEstimators.TryGetValue(type, out var estimator))
        {
            return estimator(obj);
        }

        // 检查继承关系
        foreach (var kvp in _sizeEstimators)
        {
            if (kvp.Key.IsAssignableFrom(type))
                return kvp.Value(obj);
        }
        // GameObject和Component
        if (obj is GameObject || obj is Component)
        {
            return 1024; // 1KB基础估算
        }

        // ScriptableObject
        if (obj is ScriptableObject)
        {
            return 2048; // 2KB基础估算
        }

        return 512; // 默认值
    }
}