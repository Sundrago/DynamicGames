using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// Controls the behavior of a slider UI element.
    /// </summary>
    public class SliderController : MonoBehaviour
    {
        [SerializeField] private SettingsPanel settingsPanel;
        [SerializeField] private Image knobImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Sprite offKnobImage;
        [SerializeField] private Sprite onKnobImage;

        public void OnValueChange()
        {
            UpdateSliderText();
            UpdateKnobAndTextColor();

            settingsPanel.OnVolumeChange();
        }

        private void UpdateSliderText()
        {
            var sliderValue = (int)(progressSlider.value * 10);

            if (sliderValue <= 1) valueText.text = "OFF";
            else valueText.text = (sliderValue * 10).ToString();
        }

        private void UpdateKnobAndTextColor()
        {
            if (progressSlider.value <= 0.1f / 10)
            {
                knobImage.sprite = offKnobImage;
                valueText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            }
            else
            {
                knobImage.sprite = onKnobImage;
                valueText.color = new Color(0f, 0.125f, 0.125f, 1f);
            }
        }
    }
}