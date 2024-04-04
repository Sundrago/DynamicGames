using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Controls sound effects in the game.
    /// </summary>
    public enum SfxType
    {
        LargeLaunch,
        MiddleLaunch,
        SmallLaunch
    }

    public class SoundEffectsController : SerializedMonoBehaviour
    {
        [Header("Audio Components")] 
        [SerializeField] private Dictionary<SfxType, AudioSource> sfxSources;

        public void PlaySFX(SfxType sfxType)
        {
            var audioSource = sfxSources[sfxType];
            if (DOTween.IsTweening(audioSource)) DOTween.Kill(audioSource);
            audioSource.volume = PlayerPrefs.GetFloat("settings_sfx");
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();
        }

        public void KillAllSFX()
        {
            KillSFX(SfxType.LargeLaunch);
            KillSFX(SfxType.MiddleLaunch);
            KillSFX(SfxType.SmallLaunch);
        }

        public void KillSFX(SfxType sfxType)
        {
            var audioSource = sfxSources[sfxType];
            if (audioSource.volume != 0 && !DOTween.IsTweening(audioSource))
                audioSource.DOFade(0, 1f);
        }
    }
}