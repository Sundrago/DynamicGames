using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] Slider music_slider, sfx_slider;
    [SerializeField] SFXCTRL sfx;

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % 30 == 0) {
            SetVolume();
            sfx.SetVolume();
        }
    }

    public void ShowPanel() {
        if(gameObject.activeSelf) return;
        
        AudioManager.Instance.PlaySFXbyTag(SfxTag.UI_OPEN);
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.eulerAngles = Vector3.zero;

        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

        gameObject.transform.DOLocalMoveY(-2500f, 0.5f)
            .From()
            .SetEase(Ease.OutExpo);
        gameObject.transform.DORotate(new Vector3(0f,0f,50f), 0.5f)
            .From()
            .SetEase(Ease.OutBack);
        
        music_slider.value = PlayerPrefs.GetFloat("settings_music", 0.5f);
        sfx_slider.value = PlayerPrefs.GetFloat("settings_sfx", 1f);

        music_slider.GetComponent<SliderCtrl>().OnValueChange();
        sfx_slider.GetComponent<SliderCtrl>().OnValueChange();

        gameObject.SetActive(true);
    }

    public void HidePanel() {
        AudioManager.Instance.PlaySFXbyTag(SfxTag.UI_SELECT);
        SetVolume();
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.eulerAngles = Vector3.zero;

        gameObject.transform.DOLocalMoveY(-2500f, 1.5f)
            .SetEase(Ease.OutExpo)
            .OnComplete(()=>{gameObject.SetActive(false);});
        gameObject.transform.DORotate(new Vector3(0f,0f,100f), 0.5f)
            .SetEase(Ease.OutExpo);
    }

    private void SetVolume() {
        PlayerPrefs.SetFloat("settings_music", music_slider.value);
        PlayerPrefs.SetFloat("settings_sfx", sfx_slider.value);
    }
}
