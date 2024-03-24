using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class NewHighScoreFX : MonoBehaviour
{
    [SerializeField] private GameObject particleFX;
    [SerializeField] private RectTransform title;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartFX()
    {
        DOTween.Kill(title);
        title.anchoredPosition = new Vector2(title.anchoredPosition.x, -350f);
        title.DOAnchorPosY(350, 1).SetEase(Ease.OutExpo);
        title.DOAnchorPosY(-500, 2).SetEase(Ease.InOutExpo).SetDelay(2)
            .OnComplete(()=>{gameObject.SetActive(false);});
        AudioManager.Instance.PlaySFXbyTag(SFX_tag.highScore);
        gameObject.SetActive(true);
    }
}
