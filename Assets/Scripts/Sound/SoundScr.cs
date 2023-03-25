using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundScr : MonoBehaviour
{
    public AudioMixer BGAudioMixer;

    public void SetVolume (float volume)
    {
        BGAudioMixer.SetFloat("BGVol", volume);
    }

}
