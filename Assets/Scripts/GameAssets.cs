using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{

    private static GameAssets _instance;

    [SerializeField]
    public static GameAssets instance
    {
        get
        {
            if (_instance == null) 
                _instance = Instantiate(Resources.Load("GameAssets") as GameObject).GetComponent<GameAssets>();
            return _instance;
        }

    }

    public SoundAudioClip[] soundAudioClips;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;

    }
}
