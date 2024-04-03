using System;
using System.Collections.Generic;
using MyUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.System
{
    public enum SfxTag
    {
        EnemyDeadExplosion,
        EnemyDeadBlackHole,
        EnemyDeadSpin,
        ShootTypeA,
        ShootTypeB,
        ShootTypeC,
        ShootTypeD,
        ShootTypeE,
        Blackhole,
        Spin,
        AcquiredItem,
        ShieldPop,
        RankGroup,
        RankGroupFinish,
        RankUnChanged,
        TicketOne,
        TicketTwo,
        TicketThree,
        TicketFour,
        TicketSix,
        TicketTwelve,
        TicketFail,
        TicketJackpot,
        ScoreSlider,
        TicketReap,
        TicketGen,
        GachaDrop,
        GachaRotateLever,
        GachaCapsules,
        GachaSimpleBgm,
        GachaCapsuleOpen,
        GachaNewItem,
        GachaItem,
        GachaNewOpen,
        NotEnoughMoney,
        InsertCoin,
        CoinInJar,
        AcquiredCoin,
        RocketClear,
        RocketNewLevel,
        CubeFall,
        Sparkle,
        Key,
        BlockExplode,
        Unable,
        BlockReveal,
        UI_Popup1,
        UI_Popup2,
        UI_Popup3,
        EarnedCoin,
        EarnedTicket,
        EarnedKey,
        UI_Open,
        UI_Close,
        UI_Select,
        UI_Click,
        PlayWithPet,
        HighScore,
        ShowScore,
        TicketFinish,
        TicketStart,
        Null
    }

    public class AudioManager : SerializedMonoBehaviour
    {
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private Dictionary<SfxTag, AudioData> audioDatas;

        private AudioData currentBgm;
        private SfxTag currentSfxTag = SfxTag.Blackhole;
        private float sfxVolume, bgmVolume, lastSfxPlayTime;

        public static AudioManager Instance { get; private set; }

        private void Awake() => Instance = this;
        private void Start() => AdjustVolume();

        private void AdjustVolume()
        {
            sfxVolume = PlayerData.GetFloat(DataKey.settings_sfx, 0.8f);
            bgmVolume = PlayerData.GetFloat(DataKey.settings_bgm, 0.8f);
            sfxSource.volume = sfxVolume;
            if (currentBgm == null) bgmSource.volume = bgmVolume;
            else bgmSource.volume = bgmVolume * currentBgm.volume;
        }

        public void PlaySfxByTag(SfxTag tag)
        {
            if (currentSfxTag == tag && Time.time - lastSfxPlayTime < 0.1f) return;
            lastSfxPlayTime = Time.time;
            currentSfxTag = tag;

            sfxVolume = PlayerData.GetFloat(DataKey.settings_sfx, 0.8f);
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(audioDatas[tag].src, audioDatas[tag].volume * sfxVolume);
        }

        public void PlayBGM(SfxTag tag)
        {
            bgmVolume = PlayerData.GetFloat(DataKey.settings_bgm, 0.8f);
            var data = audioDatas[tag];
            currentBgm = data;
            bgmSource.clip = data.src;
            bgmSource.volume = data.volume * bgmVolume;
            bgmSource.Play();
        }

        public void PauseBGM()
        {
            bgmSource.Pause();
        }

        public void ResumeBgm()
        {
            bgmSource.Play();
        }

#if UNITY_EDITOR
        [Button]
        private void AddSfxItems()
        {
            foreach (SfxTag sfxTag in Enum.GetValues(typeof(SfxTag)))
            {
                var alreadyHasKey = audioDatas.ContainsKey(sfxTag);
                if (!alreadyHasKey)
                {
                    var newData = new AudioData();
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
}