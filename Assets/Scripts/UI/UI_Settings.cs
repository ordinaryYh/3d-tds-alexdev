using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [Header("SFX settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] TextMeshProUGUI sfxSliderText;

    [Header("BGM settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] TextMeshProUGUI bgmSliderText;

    public void SFXSliderValue(float _value)
    {
        sfxSliderText.text = Mathf.RoundToInt(_value * 100) + "%";
    }

    public void BGMSliderValue(float _value)
    {
        bgmSliderText.text = Mathf.RoundToInt(_value * 100) + "%";
    }

    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;
    }
}
