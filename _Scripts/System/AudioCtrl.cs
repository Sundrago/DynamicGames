using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public enum SFX_tag
{
    enemy_dead_explostion, enemy_dead_blackHole, enemy_dead_spin, shootA, shootB, shootC, shootD, shootE, blackhole, spin, gotItem, shiealdPop, rank_goup, rank_goupFinish, rank_same
    , ticket1, ticket2, ticket3, ticket4, ticket6, ticket12, ticketfail, ticketJackpot, scoreSlider, reap, ticketGen, gacha_drop, gacha_rotateLever, gacha_capsules, gacha_simple_bgm
    , gachaCapsuleOpen, gacha_newItem, gacha_item, gacha_newOpen, notEnoughMoney, insertCoin, coinInJar, gotCoin, rocket_clear, rocket_newLevel, cube_fall, sparkle, key, block_explode
    , unable, block_reveal, popup1, popup2, popup3, earnCoin, earnTicket, earnKey, UI_OPEN, UI_CLOSE, UI_SELECT, click, playWithPet, highScore,
}

public class AudioCtrl : SerializedMonoBehaviour
{
    public static AudioCtrl Instance;

    [SerializeField] AudioSource sfx_source, bgm_source;

    [TableList(ShowIndexLabels = true)]
    [SerializeField] List<AudioData> audioDatas;

    private float sfxVolume = 0.8f;
    private float bgmVolume = 0.8f;
    private AudioData bgmPlaying = null;

    private void Awake()
    {
        Instance = this;
        if (!PlayerPrefs.HasKey("settings_volumeInit"))
        {
            PlayerPrefs.SetFloat("settings_sfx", 1f);
            PlayerPrefs.SetFloat("settings_bgm", 0.5f);
            PlayerPrefs.SetInt("settings_volumeInit", 1);
        }
    }

    private void Start()
    {
        SetVolume();
    }

    private float lastsfxPlayTime;
    private SFX_tag lastSfxtag = SFX_tag.blackhole;
    
    public void PlaySFXbyTag(SFX_tag tag)
    {
        if(lastSfxtag == tag && Time.time - lastsfxPlayTime < 0.15f) return;
        lastsfxPlayTime = Time.time;
        lastSfxtag = tag;
        
        sfx_source.volume = PlayerPrefs.GetFloat("settings_sfx", 1f);
        foreach(AudioData data in audioDatas)
        {
            if(data.tag == tag)
            {
                sfx_source.PlayOneShot(data.src, data.volume * sfxVolume);
            }
        }
        //sfx_source.PlayOneShot(audioClips[(int)tag]);
    }

    public void SetVolume()
    {
        sfxVolume = 1f;
        bgmVolume = PlayerPrefs.GetFloat("settings_music", 0.5f);
        sfx_source.volume = PlayerPrefs.GetFloat("settings_sfx", 1f);

        if (bgmPlaying == null) bgm_source.volume = PlayerPrefs.GetFloat("settings_music", 0.5f);
        else bgm_source.volume = PlayerPrefs.GetFloat("settings_music", 0.5f) * bgmPlaying.volume;
    }

    public void PlayBGM(SFX_tag tag)
    {
        bgmVolume = PlayerPrefs.GetFloat("settings_music", 0.5f);
        print("PLAY BGM : " + tag.ToString());
        foreach (AudioData data in audioDatas)
        {
            if (data.tag == tag)
            {
                bgmPlaying = data;
                bgm_source.clip = data.src;
                bgm_source.volume = data.volume * bgmVolume;
                bgm_source.Play();
                return;
            }
        }
    }

    public void PauseBGM()
    {
        bgm_source.Pause();
    }

    public void UnPauseBgm()
    {
        bgm_source.Play();
    }

#if UNITY_EDITOR
    [Button]
    private void AddSFXItems()
    {
        foreach (SFX_tag sfxTag in Enum.GetValues(typeof(SFX_tag)))
        {
            bool alreadyHasKey = false;
            
            for(int i = 0; i<audioDatas.Count; i++)
            {
                if (audioDatas[i].tag == sfxTag)
                {
                    alreadyHasKey = true;
                    break;
                }
            }

            if (!alreadyHasKey)
            {
                AudioData newData = new AudioData();
                newData.tag = sfxTag;
                audioDatas.Add(newData);
            }
        }
    }
#endif
    
    [Serializable]
    public class AudioData
    {
        public SFX_tag tag;
        public AudioClip src;
        [Range(0f, 1f)]
        public float volume = 0.8f;
    }

}