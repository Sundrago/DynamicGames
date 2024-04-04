using DG.Tweening;
using Utility;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.System
{
    public class SfxController : MonoBehaviour
    {
        [SerializeField] private AudioSource[] bgmSources = new AudioSource[4];
        [SerializeField] private AudioSource[] sfxSources = new AudioSource[2];
        [SerializeField] private AudioSource[] sfxSources_2 = new AudioSource[2];

        private int currentBgmIndex = -1;

        private void Start()
        {
            SetVolume();
        }

        public void PlayBGM(int idx, bool quickTransition = false, float volume = 1f)
        {
            if (idx == -1)
            {
                if (currentBgmIndex != -1)
                    FadeOutAudio(bgmSources[currentBgmIndex], 1f);
                return;
            }

            if (quickTransition)
            {
                if (currentBgmIndex != -1)
                    FadeOutAudio(bgmSources[currentBgmIndex], 1f);
                FadeInAudio(bgmSources[idx], 0.5f, volume);
                currentBgmIndex = idx;
                return;
            }

            if (currentBgmIndex != -1)
                FadeOutAudio(bgmSources[currentBgmIndex]);
            FadeInAudio(bgmSources[idx], 3f, volume);
            currentBgmIndex = idx;
        }

        public void PauseBGM()
        {
            FadeOutAudio(bgmSources[currentBgmIndex], 1f);
        }

        public void ResumeBGM()
        {
            FadeInAudio(bgmSources[currentBgmIndex], 0.5f);
        }

        public int GetCurrentBgmIndex()
        {
            return currentBgmIndex;
        }

        public void ChangeBGMVolume(float volume = 1f, float duration = 3f)
        {
            var audio = bgmSources[currentBgmIndex];
            DOTween.Kill(audio);
            audio.DOFade(PlayerData.GetFloat(DataKey.settings_bgm, 0.5f) * volume, duration);
        }

        private void FadeInAudio(AudioSource audio, float duration = 3f, float volume = 1f)
        {
            DOTween.Kill(audio);
            audio.volume = 0;
            audio.DOFade(PlayerData.GetFloat(DataKey.settings_bgm, 0.5f) * volume, duration);
            audio.Play();
        }

        private void FadeOutAudio(AudioSource audio, float duratioin = 4f)
        {
            DOTween.Kill(audio);
            audio.DOFade(0f, duratioin)
                .OnComplete(() => { audio.Stop(); });
        }

        public void PlayWaterSoundEffect()
        {
            var rnd = Random.Range(0, sfxSources.Length);
            sfxSources[rnd].Play();
        }

        public void PlaySfx(int idx)
        {
            sfxSources_2[idx].volume = PlayerData.GetFloat(DataKey.settings_sfx, 1f);
            sfxSources_2[idx].Play();
        }

        public void SetVolume()
        {
            if (currentBgmIndex != -1)
                bgmSources[currentBgmIndex].volume = PlayerData.GetFloat(DataKey.settings_bgm, 0.5f);
        }
    }
}