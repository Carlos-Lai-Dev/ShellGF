using System.IO;
using UnityEngine;

namespace ShellGF.Runtime
{
    public static class SaveManager
    {
        #region PlayerPrefs

        public static void SaveByPlayerPrefs(string key, object data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
#if UNITY_EDITOR
            Debug.Log($"{key} Save Successfully !");
#endif
        }

        public static string LoadFromPlayerPrefs(string key)
        {
            return PlayerPrefs.GetString(key);
        }
        #endregion

        #region Json

        public static void SaveByJson(string fileName, object data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                File.WriteAllText(path, json);
#if UNITY_EDITOR
                Debug.Log($"Save data to {path} Successfully !");
#endif

            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Save data to {path} Failed ! :{e}");
#endif
            }
        }

        public static T LoadFromJson<T>(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<T>(json);
                return data;
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Load data from {path} Failed ! :{e}");
#endif  
                return default;
            }
        }

        public static void DeleteSaveFile(string fileName)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                File.Delete(path);
            }
            catch (System.Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError($"Delete data from {path} Failed ! :{e}");
#endif
            }
        }
        #endregion
    }
}

