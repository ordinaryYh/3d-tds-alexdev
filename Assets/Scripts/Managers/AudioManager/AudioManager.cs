using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource[] bgm;

    [SerializeField] private bool playBgm;
    [SerializeField] private int bgmIndex;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayBGM(3);
    }

    private void Update()
    {
        if (playBgm == false && BgmIsPlaying())
            StopAllBGM();

        if (playBgm && bgm[bgmIndex].isPlaying == false)
            PlayRandomBGM();
    }

    public void PlaySFX(AudioSource sfx, bool randomPitch = false, float minPitch = 0.85f, float maxPitch = 1.1f)
    {
        if (sfx == null)
            return;

        float pitch = Random.Range(minPitch, maxPitch);

        sfx.pitch = pitch;
        sfx.Play();
    }

    public void PlaySFXWithDelayAndFade(AudioSource _source, bool _play, float _targetVolume, float _delay = 0, float _duration = 1)
    {
        StartCoroutine(SFXDelayAndFadeCo(_source, _play, _targetVolume, _delay, _duration));
    }

    public void PlayBGM(int index)
    {
        StopAllBGM();

        bgmIndex = index;
        bgm[index].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    [ContextMenu("Play Random music")]
    public void PlayRandomBGM()
    {
        StopAllBGM();
        bgmIndex = Random.Range(0, bgm.Length);

        PlayBGM(bgmIndex);
    }

    private bool BgmIsPlaying()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].isPlaying)
                return true;
        }

        return false;
    }

    private IEnumerator SFXDelayAndFadeCo(AudioSource _source, bool _play, float _targetVolume, float _delay = 0, float _duration = 1)
    {
        yield return new WaitForSeconds(_delay);

        float startVolume = _play ? 0 : _source.volume;
        float endVolume = _play ? _targetVolume : 0;
        float elapsed = 0;

        if (_play)
        {
            _source.volume = 0;
            _source.Play();
        }

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            _source.volume = Mathf.Lerp(startVolume, endVolume, elapsed / _duration);
            yield return null;
        }

        _source.volume = endVolume;

        if (_play == false)
            _source.Stop();
    }

}
