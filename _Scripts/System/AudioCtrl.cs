using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PrivateKeys;
using Sirenix.OdinInspector;

public enum SFX_tag
{
    enemy_dead_explostion, enemy_dead_blackHole, enemy_dead_spin, shootA, shootB, shootC, shootD, shootE, blackhole, spin, gotItem, shiealdPop, rank_goup, rank_goupFinish, rank_same
    , ticket1, ticket2, ticket3, ticket4, ticket6, ticket12, ticketfail, ticketJackpot, scoreSlider, reap, ticketGen, gacha_drop, gacha_rotateLever, gacha_capsules, gacha_simple_bgm
    , gachaCapsuleOpen, gacha_newItem, gacha_item, gacha_newOpen, notEnoughMoney, insertCoin, coinInJar, gotCoin, rocket_clear, rocket_newLevel, cube_fall, sparkle, key, block_explode
    , unable, block_reveal, popup1, popup2, popup3, earnCoin, earnTicket, earnKey, UI_OPEN, UI_CLOSE, UI_SELECT, click, playWithPet, highScore,showScore, ticketFin, ticketStart,
}

public class AudioCtrl : SerializedMonoBehaviour
{
    public static AudioCtrl Instance;

    [SerializeField] AudioSource sfx_source, bgm_source;
    [SerializeField] private Dictionary<SFX_tag, AudioData> audioDatas;
    
    private float sfxVolume = 0.8f;
    private float bgmVolume = 0.8f;
    private AudioData bgmPlaying = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetVolume();
    }

    private float lastsfxPlayTime;
    private SFX_tag lastSfxtag = SFX_tag.blackhole;
    
    public void PlaySFXbyTag(SFX_tag tag)
    {
        if(lastSfxtag == tag && Time.time - lastsfxPlayTime < 0.1f) return;
        lastsfxPlayTime = Time.time;
        lastSfxtag = tag;
        
        sfx_source.volume = PlayerPrefs.GetFloat(PlayerData.SFX_VOLIME, 1f);
        sfx_source.PlayOneShot(audioDatas[tag].src, audioDatas[tag].volume * sfxVolume);
    }

    private void SetVolume()
    {
        sfxVolume = 1f;
        bgmVolume = PlayerPrefs.GetFloat(PlayerData.BGM_VOLUME, 0.8f);
        sfx_source.volume = PlayerPrefs.GetFloat(PlayerData.SFX_VOLIME, 0.8f);

        if (bgmPlaying == null) bgm_source.volume = PlayerPrefs.GetFloat(PlayerData.BGM_VOLUME, 0.8f);
        else bgm_source.volume = PlayerPrefs.GetFloat(PlayerData.BGM_VOLUME, 0.8f) * bgmPlaying.volume;
    }

    public void PlayBGM(SFX_tag tag)
    {
        bgmVolume = PlayerPrefs.GetFloat(PlayerData.BGM_VOLUME, 0.8f);
        AudioData data = audioDatas[tag];
        bgmPlaying = data;
        bgm_source.clip = data.src;
        bgm_source.volume = data.volume * bgmVolume;
        bgm_source.Play();
        return;
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
            bool alreadyHasKey = audioDatas.ContainsKey(sfxTag);
            if (!alreadyHasKey)
            {
                AudioData newData = new AudioData();
                audioDatas.Add(sfxTag, newData);
            }
        }
    }
#endif
    
    [Serializable]
    public class AudioData
    {
        public AudioClip src;
        [Range(0f, 1f)]
        public float volume = 0.8f;
    }

}