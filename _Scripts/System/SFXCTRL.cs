using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SFXCTRL : MonoBehaviour
{
    [SerializeField] AudioSource[] bgms = new AudioSource[4];
    [SerializeField] AudioSource[] sfxs = new AudioSource[2];
    [SerializeField] AudioSource[] sfxs_2 = new AudioSource[2];

    private int currentBgm = -1;

    void Start()
    {
        if(!PlayerPrefs.HasKey("settings_music")) {
            PlayerPrefs.SetFloat("settings_music", 1f);
            PlayerPrefs.SetFloat("settings_sfx", 0.5f);
        }

        SetVolume();
    }

    public void PlayBGM(int idx, bool shortTransition = false, float volume = 1f) {
        print("bgm idx : " +idx );

        if (idx == -1)
        {
            if(currentBgm != -1)    
                AudioOut(bgms[currentBgm], 1f);
            return;
        }
        
        if(shortTransition) {
            if(currentBgm != -1)    
            AudioOut(bgms[currentBgm], 1f);
            AudioIn(bgms[idx], 0.5f, volume);
            currentBgm = idx;
            return;
        }

        if(currentBgm != -1)    
            AudioOut(bgms[currentBgm]);
        AudioIn(bgms[idx], 3f, volume);
        currentBgm = idx;
    }

    public int GetCurrentBgm() {
        return currentBgm;
    }

    public void ChangeBGMVolume(float volume = 1f, float duration = 3f)
    {
        AudioSource audio = bgms[currentBgm];
        DOTween.Kill(audio);
        audio.DOFade(PlayerPrefs.GetFloat("settings_music") * volume, duration);
    }

    private void AudioIn(AudioSource audio, float duration = 3f, float volume = 1f)
    {
        DOTween.Kill(audio);
        audio.volume = 0;
        audio.DOFade(PlayerPrefs.GetFloat("settings_music") * volume, duration);
        audio.Play();
    }

    private void AudioOut(AudioSource audio, float duratioin = 4f) {
        DOTween.Kill(audio);
        audio.DOFade(0f, duratioin)
            .OnComplete(()=>{audio.Stop();});
    }

    public void WaterSfx() {
        int rnd = Random.Range(0, sfxs.Length);
        sfxs[rnd].Play();
    }

    public void PlaySfx(int idx) {
        sfxs_2[idx].volume = PlayerPrefs.GetFloat("settings_sfx");
        sfxs_2[idx].Play();
    }

    public void SetVolume() {
        if(currentBgm != -1)
            bgms[currentBgm].volume = PlayerPrefs.GetFloat("settings_music");
    }
}
