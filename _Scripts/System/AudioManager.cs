using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyUtility;
using Sirenix.OdinInspector;

public enum SfxTag
{
    enemy_dead_explostion, enemy_dead_blackHole, enemy_dead_spin, shootA, shootB, shootC, shootD, shootE, blackhole, spin, gotItem, shiealdPop, rank_goup, rank_goupFinish, rank_same
    , ticket1, ticket2, ticket3, ticket4, ticket6, ticket12, ticketfail, ticketJackpot, scoreSlider, reap, ticketGen, gacha_drop, gacha_rotateLever, gacha_capsules, gacha_simple_bgm
    , gachaCapsuleOpen, gacha_newItem, gacha_item, gacha_newOpen, notEnoughMoney, insertCoin, coinInJar, gotCoin, rocket_clear, rocket_newLevel, cube_fall, sparkle, key, block_explode
    , unable, block_reveal, popup1, popup2, popup3, earnCoin, earnTicket, earnKey, UI_OPEN, UI_CLOSE, UI_SELECT, click, playWithPet, highScore,showScore, ticketFin, ticketStart,Null, 
}

public class AudioManager : SerializedMonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioSource sfx_source, bgm_source;
    [SerializeField] private Dictionary<SfxTag, AudioData> audioDatas;

    private float sfxVolume = 0.8f;
    private float bgmVolume = 0.8f;
    private AudioData bgmPlaying = null;

    private float lastsfxPlayTime;
    private SfxTag lastSfxtag = SfxTag.blackhole;

    private void Awake() => Instance = this;

    private void Start()
    {
        SetVolume();
    }

    private void SetVolume()
    {
        bgmVolume = PlayerData.GetFloat(DataKey.settings_bgm, 0.8f);
        sfxVolume = PlayerData.GetFloat(DataKey.settings_sfx, 0.8f);

        sfx_source.volume = sfxVolume;
        if (bgmPlaying == null) bgm_source.volume = bgmVolume;
        else bgm_source.volume = bgmVolume * bgmPlaying.volume;
    }

    public void PlaySFXbyTag(SfxTag tag)
    {
        if (lastSfxtag == tag && Time.time - lastsfxPlayTime < 0.1f) return;
        lastsfxPlayTime = Time.time;
        lastSfxtag = tag;

        sfxVolume = PlayerData.GetFloat(DataKey.settings_sfx, 0.8f);
        sfx_source.volume = sfxVolume;
        sfx_source.PlayOneShot(audioDatas[tag].src, audioDatas[tag].volume * sfxVolume);
    }

    public void PlayBGM(SfxTag tag)
    {
        bgmVolume = PlayerData.GetFloat(DataKey.settings_bgm, 0.8f);
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
        foreach (SfxTag sfxTag in Enum.GetValues(typeof(SfxTag)))
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
        [Range(0f, 1f)] public float volume = 0.8f;
    }
}