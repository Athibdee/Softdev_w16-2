using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DontDestroyA : MonoBehaviour
{
    public AudioMixer BGAudioMixer;
    float PlayerVolume;

    public void Start()
    {
        PlayerVolume = PlayerPrefs.GetFloat("save", 0);
        BGAudioMixer.SetFloat("BGVol", PlayerVolume);
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

}
