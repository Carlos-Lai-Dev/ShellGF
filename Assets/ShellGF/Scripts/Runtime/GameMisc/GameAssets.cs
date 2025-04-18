using System;
using UnityEngine;

namespace ShellGF.Runtime
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets instance;
        public static GameAssets Instance => instance != null ? instance : instance = GetGameRoot();
        public SoundAudioClip[] soundAudioClip_Arr;

        [Serializable]
        public class SoundAudioClip
        {
            public SoundName soundName;
            public AudioData[] audioData;
        }
        private static GameAssets GetGameRoot()
        {
            var gameObject = Instantiate(ABManager.GetInstance().LoadRes<GameObject>(ABName.gameasset, "GameAssets"));
            var gameAssets = gameObject.GetComponent<GameAssets>();
            return gameAssets;
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    [Serializable]
    public class AudioData
    {
        public AudioClip audioClip;
        public float volume;
    }
}
