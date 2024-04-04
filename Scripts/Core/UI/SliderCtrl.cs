using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.UI
{
    public class SliderCtrl : MonoBehaviour
    {
        [SerializeField] private SettingsPanel settingsPanel;

        [FormerlySerializedAs("knob")] [SerializeField]
        private Image knobImage;

        [FormerlySerializedAs("text")] [SerializeField]
        private TextMeshProUGUI valueText;

        [FormerlySerializedAs("slider")] [SerializeField]
        private Slider progressSlider;

        [FormerlySerializedAs("off")] [SerializeField]
        private Sprite offKnobImage;

        [FormerlySerializedAs("on")] [SerializeField]
        private Sprite onKnobImage;

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