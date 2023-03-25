using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderCon : MonoBehaviour
{
    public Slider slider;

    public float sliderValue;

    public void Start()
    {
        slider.value = PlayerPrefs.GetFloat("save", sliderValue);
    }

    public void ChangeSlider(float value)
    {
        sliderValue = value;
        PlayerPrefs.SetFloat("save", sliderValue);
    }
}
