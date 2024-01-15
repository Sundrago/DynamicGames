using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;


public class ItemInfo : MonoBehaviour
{
    [SerializeField] private Image itemImage, itemImageBright, descr;

    [SerializeField] private TextMeshProUGUI itemCount_ui, descr_ui;

    private float startTime, totalTime = -1;
    private int itemCount;
    
    [Button]
    public void Init(float _timer, int _itemCount = 0)
    {
        totalTime = _timer;
        itemCount = _itemCount;
        itemImageBright.fillAmount = 1;
        startTime = Time.time;

        itemImage.DOFade(0.15f, 0.5f);
        itemImageBright.DOFade(0.25f, 0.5f);

        if (itemCount == 0)
        {
            itemCount_ui.gameObject.SetActive(false);
        }
        else
        {
            itemCount_ui.text = itemCount.ToString();
            itemCount_ui.DOFade(0.5f, 0.5f);
            itemCount_ui.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if(totalTime == -1) return;

        float timespan = totalTime - (Time.time - startTime);
        if (timespan > 1.5f)
        {
            itemImageBright.fillAmount = (timespan - 1.5f) / (totalTime - 1.5f);
        } else if (timespan > 0f)
        {
            itemImage.color = new Color(1, 1, 1, (Mathf.Sin(Mathf.PI * 4f * timespan)/4f + 0.25f))/2f;
        }
        else
        {
            //hide
            gameObject.SetActive(false);
        }
    }

    public void ShowDescr()
    {
        DOTween.Kill(descr);
        DOTween.Kill(descr_ui);
        descr.gameObject.SetActive(true);
        descr_ui.gameObject.SetActive(true);
        descr.DOFade(0.3f, 0.5f);
        descr_ui.DOFade(0.7f, 0.5f);
        descr.DOFade(0f, 1f).SetDelay(3f);
        descr_ui.DOFade(0f, 1f).SetDelay(3f);;
    }

    private void Start()
    {
        Hide();
    }
    
    public void Hide()
    {
        DOTween.Kill(descr);
        DOTween.Kill(descr_ui);
        DOTween.Kill(itemImage);
        DOTween.Kill(itemImageBright);
        itemImage.DOFade(0, 0.5f);
        itemImageBright.DOFade(0, 0.5f);
        descr.DOFade(0, 0.5f);
        descr_ui.DOFade(0, 0.5f);
        itemCount_ui.text = "";
    }
}
