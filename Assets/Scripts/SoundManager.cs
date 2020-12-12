using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        buttonClick,
        jump,
        whip,
        hurt,
        collect,
        tikaDie,
        tikaAlert,
        eskimoDie,
        eskimoAlert,
        mummyDie,
        mummyAlert,
        bossDie
    }

    private static Dictionary<Sound, float> soundTimerDictionary;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.tikaAlert] = 0f;
        soundTimerDictionary[Sound.jump] = 0f;
    }

    public static void PlaySound(Sound s)
    {
        if (CanPlaySound(s))
        {
            GameObject soundGameObject = new GameObject("Sound :" + s);
            AudioSource source = soundGameObject.AddComponent<AudioSource>();
            AudioClip clip = GetAudioClip(s);
            source.PlayOneShot(clip);
            Destroy(soundGameObject, clip.length);
        }
    }

    public static void PlaySound(Sound s, Vector3 position)
    {
        if (CanPlaySound(s))
        {
            GameObject soundGameObject = new GameObject("Sound :" + s);
            soundGameObject.transform.position = position;
            AudioSource source = soundGameObject.AddComponent<AudioSource>();
            source.clip = GetAudioClip(s);
            source.Play();
            Destroy(soundGameObject, source.clip.length);
        }
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.tikaAlert:
                return CheckWaitToPlay(5f, sound);
            case Sound.jump:
                return CheckWaitToPlay(0.5f, sound);
        }
    }

    private static bool CheckWaitToPlay(float waitTime, Sound sound)
    {
        if (soundTimerDictionary.ContainsKey(sound))
        {
            float lastTimePlayed = soundTimerDictionary[sound];
            float alertSoundTimerMax = waitTime;
            if (lastTimePlayed + alertSoundTimerMax < Time.time)
            {
                soundTimerDictionary[sound] = Time.time;

                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return true;
    }

    private static AudioClip GetAudioClip(Sound s)
    {
        Debug.Log("The soundAudioClip contains : "+ GameAssets.instance.soundAudioClips.Length);
        foreach(GameAssets.SoundAudioClip soundAudioClip in GameAssets.instance.soundAudioClips)
        {
            if (soundAudioClip.sound == s)
                return soundAudioClip.audioClip;
        }
        Debug.LogError("ERROR: Sound " + s + " not found");
        return null;
    }
}
