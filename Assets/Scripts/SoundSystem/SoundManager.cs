using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundName
{
    PlayerMove,
    PlayerAttack,
    EnemyHit,
    EnemyDie,
    Treasure,
    BtnClick,
    StartBGM,
    GameBGM,
}
public static class SoundManager
{
    private static GameObject oneShot_GO;
    private static AudioSource oneShot_AS;
    private static GameObject BGM_GO;
    private static AudioSource BGM_AS;
    private static GameObject soundPrefab = null;
    private readonly static float min_Volume = 0.9f;
    private readonly static float max_Volume = 1.1f;
    private static Dictionary<SoundName, float> soundTimer_Dic;
    private class Mono : MonoBehaviour { }
    public static void Init()
    {
        soundTimer_Dic = new Dictionary<SoundName, float>()
        {
            [SoundName.PlayerMove] = 0f
        };
        ABManager.GetInstance().LoadResAsync<GameObject>(ABName.pool, "Sound", (go) =>
        {
            soundPrefab = go;
        });

        SetBGM(SoundName.StartBGM);
    }

    public static void SetBGM(SoundName soundName)
    {
        if (BGM_GO == null)
        {
            BGM_GO = new GameObject("BGM");
            GameObject.DontDestroyOnLoad(BGM_GO);
            BGM_AS = BGM_GO.AddComponent<AudioSource>();
            BGM_AS.loop = true;
        }
        var AD = GetAudioData(soundName);
        BGM_AS.clip = AD.audioClip;
        BGM_AS.volume = AD.volume == 0 ? 1 : AD.volume;
        BGM_AS.Play();
    }
    private static GameObject CreateAudioSource_GO(Vector3 pos)
    {
        var soundObject = PoolManager.Release(soundPrefab, pos);
        var audioSource = soundObject.GetOrAddComponent<AudioSource>();
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        return soundObject;
    }
    static IEnumerator AutoDeactivate(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }
    public static void PlaySound3D(SoundName soundName, Vector3 pos)
    {
        if (CanPlaySound(soundName))
        {
            var soundObj = CreateAudioSource_GO(pos);
            var audioSource = soundObj.GetOrAddComponent<AudioSource>();
            var audioData = GetAudioData(soundName);
            audioSource.PlaySFX(audioData);

            soundObj.GetOrAddComponent<Mono>().StartCoroutine(AutoDeactivate(soundObj, audioData.audioClip.length));

        }
    }
    public static void PlaySound3DRandomPitch(SoundName soundName, Vector3 pos)
    {
        if (CanPlaySound(soundName))
        {
            var soundObject = CreateAudioSource_GO(pos);
            var audioSource = soundObject.GetOrAddComponent<AudioSource>();
            var audioData = GetAudioData(soundName);
            audioSource.PlaySFXRandomPitch(audioData);

            soundObject.GetOrAddComponent<Mono>().StartCoroutine(AutoDeactivate(soundObject, audioData.audioClip.length));
        }
    }
    private static void CreateOneShot_GO()
    {
        if (oneShot_GO == null)
        {
            oneShot_GO = new GameObject("One Shot Sound");
            oneShot_AS = oneShot_GO.AddComponent<AudioSource>();
            GameObject.DontDestroyOnLoad(oneShot_GO);
        }
    }
    public static void PlaySound(SoundName soundName)
    {
        if (CanPlaySound(soundName))
        {
            CreateOneShot_GO();
            oneShot_AS.PlaySFX(GetAudioData(soundName));
        }
    }

    public static void PlaySoundRandomPitch(SoundName soundName)
    {
        if (CanPlaySound(soundName))
        {
            CreateOneShot_GO();
            oneShot_AS.PlaySFXRandomPitch(GetAudioData(soundName));
        }
    }
    public static void PlaySFX(this AudioSource audioSource, AudioData audioData)
    {
        audioSource.PlayOneShot(audioData.audioClip, audioData.volume == 0 ? 1 : audioData.volume);
    }

    public static void PlaySFXRandomPitch(this AudioSource audioSource, AudioData audioData)
    {
        audioSource.pitch = Random.Range(min_Volume, max_Volume);
        audioSource.PlayOneShot(audioData.audioClip, audioData.volume == 0 ? 1 : audioData.volume);
    }
    private static bool CanPlaySound(SoundName soundName)
    {
        switch (soundName)
        {
            default:
                return true;
            case SoundName.PlayerMove:
                if (soundTimer_Dic.ContainsKey(soundName))
                {
                    float lastTimePlayed = soundTimer_Dic[soundName];
                    float playerMoveTimeMax = 0.15f;
                    if (lastTimePlayed + playerMoveTimeMax < Time.time)
                    {
                        soundTimer_Dic[soundName] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
        }
    }
    private static AudioData GetAudioData(SoundName soundName)
    {
        foreach (var soundAudioClip in GameAssets.Instance.soundAudioClip_Arr)
        {
            if (soundAudioClip.soundName == soundName)
            {
                return soundAudioClip.audioData[Random.Range(0, soundAudioClip.audioData.Length)];
            }
        }
#if UNITY_EDITOR
        Debug.Log($"Sound {soundName} not found !");
#endif
        return null;
    }


}
