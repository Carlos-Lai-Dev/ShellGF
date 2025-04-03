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
            long size = 1024; // ������С

            // ����shader���������ӹ���
            if (material.shader != null)
            {
                size += material.shader.passCount * 512;
            }

            // ��������
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
            // ���㹫ʽ�������� �� ͨ���� �� λ��/8
            int channels = audioClip.channels;
            int bitDepth = 16; // ����16λ
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
            // ��������
            long size = mesh.vertexCount * 12; // ����ÿ������12�ֽ�(x,y,z �� 4�ֽ�float)

            // ��������
            if (mesh.indexFormat == UnityEngine.Rendering.IndexFormat.UInt32)
                size += mesh.triangles.Length * 4;
            else
                size += mesh.triangles.Length * 2;

            // ��������(���ߡ�UV��)
            if (mesh.normals != null && mesh.normals.Length > 0)
                size += mesh.vertexCount * 12;

            if (mesh.uv != null && mesh.uv.Length > 0)
                size += mesh.vertexCount * 8;

            // ��Ƥ����������
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
            // �������㣺�� �� �� �� ÿ�����ֽ���
            int bytesPerPixel = 4; // Ĭ�ϰ�RGBA32��ʽ

            // ����ʵ�ʸ�ʽ����
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
                    // ѹ������1�ֽ�/���ع���
                    bytesPerPixel = 1; break;

                default:
                    bytesPerPixel = 4; break;
            }

            // ����mipmaps
            float mipmapFactor = texture.mipmapCount > 1 ? 1.33f : 1f;

            return (long)(texture.width * texture.height * bytesPerPixel * mipmapFactor);
        }
        return 0;
    }

    private static long EstimateMemoryUsage(UnityEngine.Object obj)
    {
        // �򵥹�������ڴ�ռ��
        if (obj == null) return 0;

        var type = obj.GetType();
        if (_sizeEstimators.TryGetValue(type, out var estimator))
        {
            return estimator(obj);
        }

        // ���̳й�ϵ
        foreach (var kvp in _sizeEstimators)
        {
            if (kvp.Key.IsAssignableFrom(type))
                return kvp.Value(obj);
        }
        // GameObject��Component
        if (obj is GameObject || obj is Component)
        {
            return 1024; // 1KB��������
        }

        // ScriptableObject
        if (obj is ScriptableObject)
        {
            return 2048; // 2KB��������
        }

        return 512; // Ĭ��ֵ
    }
}