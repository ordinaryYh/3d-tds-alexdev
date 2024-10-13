using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float sliderMultiplier = 25;

    [Header("SFX settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] TextMeshProUGUI sfxSliderText;
    [SerializeField] private string sfxParameter;

    [Header("BGM settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] TextMeshProUGUI bgmSliderText;
    [SerializeField] private string bgmParameter;

    [Header("Toggle")]
    [SerializeField] private Toggle friendlyFireToggle;


    public void SFXSliderValue(float _value)
    {
        sfxSliderText.text = Mathf.RoundToInt(_value * 100) + "%";
        float newValue = Mathf.Log10(_value) * sliderMultiplier;
        audioMixer.SetFloat(sfxParameter, newValue);
    }

    public void BGMSliderValue(float _value)
    {
        bgmSliderText.text = Mathf.RoundToInt(_value * 100) + "%";
        float newValue = Mathf.Log10(_value) * sliderMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
    }

    public void OnFriendlyFireToggle()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        GameManager.instance.friendlyFire = !friendlyFire;
    }

    public void LoadValues()
    {
        int fridenlyFire = PlayerPrefs.GetInt("FriendlyFire", 0);

        bool _newFridenlyFire = false;
        if (fridenlyFire == 1)
            _newFridenlyFire = true;

        friendlyFireToggle.isOn = _newFridenlyFire;


        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.7f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.7f);
    }

    private void OnDisable()
    {
        bool friendlyFire = GameManager.instance.friendlyFire;
        int friendlyFireInt = friendlyFire ? 1 : 0;

        PlayerPrefs.SetInt("FriendlyFire", friendlyFireInt);
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }
}
