using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SliderCtrl : MonoBehaviour
{
    [SerializeField] Image knob; 
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider slider;
    [SerializeField] Sprite off, on;

    public void OnValueChange() {
        if(slider.value > 10f/11f) {
            text.text = "100";
        } else if(slider.value > 9f/11f) {
            text.text = "90";
        }else if(slider.value > 8f/11f) {
            text.text = "80";
        }else if(slider.value > 7f/11f) {
            text.text = "70";
        }else if(slider.value > 6f/11f) {
            text.text = "60";
        }else if(slider.value > 5f/11f) {
            text.text = "50";
        }else if(slider.value > 4f/11f) {
            text.text = "40";
        }else if(slider.value > 3f/11f) {
            text.text = "30";
        }else if(slider.value > 2f/11f) {
            text.text = "20";
        }else if(slider.value > 0.1f/11f) {
            text.text = "10";
        } else {
            text.text = "OFF";
        }

        if(slider.value <= 0.1f/11) {
            knob.sprite = off;
            text.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        } else {
            knob.sprite = on;
            text.color = new Color(0f, 0.125f, 0.125f, 1f);
        }
    }
}