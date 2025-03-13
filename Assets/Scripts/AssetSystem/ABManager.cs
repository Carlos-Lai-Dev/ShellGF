using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABManager : SingletonMono<ABManager>
{
    private static readonly string basePath = Application.streamingAssetsPath + '/';
    private static readonly string mainAB_Name =
#if UNITY_ANDROID
             "Android";
#elif UNITY_IOS
             "IOS";
#elif UNITY_STANDALONE_WIN
             "Windows";
#endif
    private AssetBundleManifest manifest;
    private enum ABStatus { Completed, Loading, NotLoaded, }
    private readonly Dictionary<string, AssetBundle> assetBundle_Dic = new Dictionary<string, AssetBundle>();
    private readonly Dictionary<string, ABStatus> loadStatus_Dic = new Dictionary<string, ABStatus>();

    private void Awake()
    {
        AssetBundle main_AB = AssetBundle.LoadFromFile(basePath + mainAB_Name);
        manifest = main_AB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        main_AB.Unload(false);
    }

    public void UnLoad(string abName, bool unLoadAllLoadedObjects = false)
    {
        if (!assetBundle_Dic.ContainsKey(abName) || assetBundle_Dic[abName] == null) return;
        assetBundle_Dic[abName].Unload(unLoadAllLoadedObjects);
        assetBundle_Dic.Remove(abName);
        loadStatus_Dic.Remove(abName);
    }

    public void UnLoadAllAssetBundles(bool unLoadAllLoadedObjects = false)
    {
        assetBundle_Dic.Clear();
        loadStatus_Dic.Clear();
        AssetBundle.UnloadAllAssetBundles(unLoadAllLoadedObjects);
    }

    private ABStatus CheckStatus(string abName) => loadStatus_Dic.TryGetValue(abName, out ABStatus value) ? value : ABStatus.NotLoaded;
    private void SetStatus(string abName, ABStatus status) => loadStatus_Dic[abName] = status;

    private void LoadAssetBundle(string abName)
    {
        Queue<string> load_Queue = new Queue<string>();
        load_Queue.Enqueue(abName);
        for (; load_Queue.Count > 0; load_Queue.Dequeue())
        {
            string name = load_Queue.Peek();
            if (CheckStatus(name) == ABStatus.Completed) continue;
            if (CheckStatus(name) == ABStatus.Loading) UnLoad(name);

            assetBundle_Dic[name] = AssetBundle.LoadFromFile(basePath + name);
#if UNITY_EDITOR
            if (assetBundle_Dic[name] == null)
            {
                throw new ArgumentException($"AssetBundle: '{name}' load fail ! ");
            }
#endif
            SetStatus(name, ABStatus.Completed);

            foreach (var depend in manifest.GetAllDependencies(name))
            {
                load_Queue.Enqueue(depend);
            }

        }
    }

    public T LoadRes<T>(string abName, string resName) where T : UnityEngine.Object
    {
        if (CheckStatus(abName) != ABStatus.Completed) LoadAssetBundle(abName);
        T res = assetBundle_Dic[abName].LoadAsset<T>(resName);
#if UNITY_EDITOR
        if (res == null)
        {
            throw new ArgumentException($"'{resName}' In AssetBundle '{abName}' Can't found !");
        }
#endif
        return res;
    }

    private IEnumerator LoadAssetBundleAsync(string abName)
    {
        HashSet<string> visitedBundle_Hash = new HashSet<string>() { abName };
        Stack<(string, bool)> load_Stack = new Stack<(string, bool)>();
        load_Stack.Push((abName, true));
        while (load_Stack.Count > 0)
        {
            var (name, needAddDepends) = load_Stack.Peek();
            if (CheckStatus(name) == ABStatus.Completed)
            { 
                load_Stack.Pop();
                continue;
            }

            if (CheckStatus(name) == ABStatus.Loading)
            { 
                yield return null;
                continue;
            }

            if(needAddDepends)
            {
                load_Stack.Pop();
                load_Stack.Push((name, false));
                foreach(var depend in manifest.GetAllDependencies(name))
                {
                    if (visitedBundle_Hash.Add(depend))
                    {
                        load_Stack.Push((depend, true));   
                    }
                }
                continue;
            }
            AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromFileAsync(basePath + name);
            assetBundle_Dic[name] = bundleCreateRequest.assetBundle;
            SetStatus(name,ABStatus.Loading);
#if UNITY_EDITOR
            if(assetBundle_Dic[name] == null)
            {
                throw new ArgumentException($"AssetBundle '{name}' Load fail !");
            }
#endif
            yield return bundleCreateRequest;
            SetStatus(name, ABStatus.Completed);    
        }
    }
}
