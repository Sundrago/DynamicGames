using Core.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;

namespace Core.UI
{
    public class SettingsPanel : MonoBehaviour, IPanelObject
    {
        [FormerlySerializedAs("musicSlider")] [FormerlySerializedAs("music_slider")] [SerializeField]
        private Slider bgmSlider;

        [FormerlySerializedAs("sfx_slider")] [SerializeField]
        private Slider sfxSlider;

        [FormerlySerializedAs("sfx")] [SerializeField]
        private SfxController sfxController;

        public void ShowPanel()
        {
            if (gameObject.activeSelf) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Open);

            SetupPanelAnimation();
            UpdateVolumeSlider();
        }

        public void HidePanel()
        {
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Select);
            SetVolume();
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.eulerAngles = Vector3.zero;

            gameObject.transform.DOLocalMoveY(-2500f, 1.5f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() => { gameObject.SetActive(false); });
            gameObject.transform.DORotate(new Vector3(0f, 0f, 100f), 0.5f)
                .SetEase(Ease.OutExpo);
        }

        public void OnVolumeChange()
        {
            SetVolume();
            sfxController.SetVolume();
        }

        private void SetupPanelAnimation()
        {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.eulerAngles = Vector3.zero;

            if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
            gameObject.transform.DOLocalMoveY(-2500f, 0.5f)
                .From()
                .SetEase(Ease.OutExpo);
            gameObject.transform.DORotate(new Vector3(0f, 0f, 50f), 0.5f)
                .From()
                .SetEase(Ease.OutBack);

            gameObject.SetActive(true);
        }

        private void UpdateVolumeSlider()
        {
            if (bgmSlider.value != PlayerData.GetFloat(DataKey.settings_bgm, 0.5f))
                bgmSlider.value = PlayerData.GetFloat(DataKey.settings_bgm, 0.5f);

            if (sfxSlider.value != PlayerData.GetFloat(DataKey.settings_sfx, 1f))
                sfxSlider.value = PlayerData.GetFloat(DataKey.settings_sfx, 1f);
        }

        private void SetVolume()
        {
            Debug.Log(bgmSlider.value + ", " + sfxSlider.value);
            PlayerData.SetFloat(DataKey.settings_bgm, bgmSlider.value);
            PlayerData.SetFloat(DataKey.settings_sfx, sfxSlider.value);
        }
    }
}