using System;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public static GameAssets Instance => instance ??= Instantiate(Resources.Load<GameAssets>("Prefabs/GameAssets/GameAssets"));
    public SoundAudioClip[] soundAudioClip_Arr;

    [Serializable]
    public class SoundAudioClip
    { 
        public SoundName soundName;
        public AudioData[] audioData;
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