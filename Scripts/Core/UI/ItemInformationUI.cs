using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class ItemInformationUI : MonoBehaviour
{
    [SerializeField] private Image itemImage, itemImageBright, descr;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private float startDisplayTime, totalDisplayTime = -1;
    private int itemQuantity;
    
    private void Start()
    {
        HideUIElements();
    }
    
    private void Update()
    {
        if(totalDisplayTime == -1) return;

        float timespan = totalDisplayTime - (Time.time - startDisplayTime);
        if (timespan > 1.5f)
        {
            itemImageBright.fillAmount = (timespan - 1.5f) / (totalDisplayTime - 1.5f);
        } else if (timespan > 0f)
        {
            itemImage.color = new Color(1, 1, 1, (Mathf.Sin(Mathf.PI * 4f * timespan)/4f + 0.25f))/2f;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void Init(float totalDisplayTime, int itemQuantity = 0)
    {
        this.totalDisplayTime = totalDisplayTime;
        this.itemQuantity = itemQuantity;
        itemImageBright.fillAmount = 1;
        startDisplayTime = Time.time;

        itemImage.DOFade(0.15f, 0.5f);
        itemImageBright.DOFade(0.25f, 0.5f);

        if (itemQuantity == 0)
        {
            itemCountText.gameObject.SetActive(false);
        }
        else
        {
            itemCountText.text = itemQuantity.ToString();
            itemCountText.DOFade(0.5f, 0.5f);
            itemCountText.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }


    public void ShowUI()
    {
        DOTween.Kill(descr);
        DOTween.Kill(descriptionText);
        descr.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        descr.DOFade(0.3f, 0.5f);
        descriptionText.DOFade(0.7f, 0.5f);
        descr.DOFade(0f, 1f).SetDelay(3f);
        descriptionText.DOFade(0f, 1f).SetDelay(3f);;
    }
    
    private void HideUIElements()
    {
        itemImage.color = GetTransparentColor(itemImage.color);
        itemImageBright.color = GetTransparentColor(itemImageBright.color);
        descr.color = GetTransparentColor(descr.color);
        descriptionText.color = GetTransparentColor(descriptionText.color);
        itemCountText.text = "";

        Color GetTransparentColor(Color color)
        {
            Color temp = color;
            temp.a = 0;
            return temp;
        }
    }
    
    public void HideUI()
    {
        DOTween.Kill(descr);
        DOTween.Kill(descriptionText);
        DOTween.Kill(itemImage);
        DOTween.Kill(itemImageBright);
        itemImage.DOFade(0, 0.5f);
        itemImageBright.DOFade(0, 0.5f);
        descr.DOFade(0, 0.5f);
        descriptionText.DOFade(0, 0.5f);
        itemCountText.text = "";
    }
}
