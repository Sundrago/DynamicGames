using System;
using System.Collections;
using System.Collections.Generic;
using Core.Pet;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEditor;

public class WatchAdsContinue : MonoBehaviour
{
    // public float normal;

    [SerializeField] private RectMask2D rectMask2D;
    [SerializeField] private Button yesBtn;
    [SerializeField] private RectTransform rect, rect2;
    [SerializeField] private UIPetSpriteAnimator petAnim, petAnim2;
    [SerializeField] private Image bg;

    private float startTime;
    private const float duration = 3f;
    
    public delegate void Callback();
    private Callback callbackYes, callbackNo;

    private bool isActive = false;
    private bool isInvisBtnDown;

    public static WatchAdsContinue Instance;
    private string note;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isActive) return;

        if (isInvisBtnDown) startTime -= Time.deltaTime * 1.5f;
        
        if (Time.time > startTime + duration)
        {
            if (callbackNo != null)
                callbackNo();
            isActive = false;
            Hide();
            yesBtn.interactable = false;
            return;
        }
        float normal = (duration - Time.time + startTime) / duration;
        UpdateProgressBar(normal);
    }

    private void UpdateProgressBar(float normal)
    {
        normal = normal * normal;
        rectMask2D.padding = new Vector4(0, 0, rectMask2D.GetComponent<RectTransform>().sizeDelta.x * (1-normal), 0);
    }

    [Button]
    public void Init(Callback _callbackYes, Callback _callbackNo, string _note)
    {
        note = _note;
        if (!PetInGameManager.Instance.enterGameWithPet)
        {
            _callbackNo();
            return;
        }
        
        callbackYes = _callbackYes;
        callbackNo = _callbackNo;
        isActive = false;
        yesBtn.interactable = false;
        UpdateProgressBar(1);
        isInvisBtnDown = false;
        
        DOTween.Kill(rect);
        rect.anchoredPosition = new Vector2(0f, -500f);
        rect.DOAnchorPosY(150, 0.5f).SetEase(Ease.OutExpo);
        rect.DOPunchRotation(Vector3.one * 3f, 0.55f).OnComplete(() =>
        {
            isActive = true;
            yesBtn.interactable = true;
            startTime = Time.time;
        });
        
        //PetMotion
        petAnim.Init(PetInGameManager.Instance.pet.type);
        gameObject.SetActive(true);
        
        //bg
        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.3f, 1f);
        
        rect.gameObject.SetActive(true);
        rect2.gameObject.SetActive(false);
    }

    private void Hide()
    {
        DOTween.Kill(rect);
        bg.DOFade(0f, 0.5f);
        rect.DOAnchorPosY(-500f, 0.4f).SetEase(Ease.OutExpo);
        rect.DOPunchRotation(Vector3.one * 3f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void YesBtnClicked()
    {
        DOTween.Kill(rect);
        yesBtn.interactable = false;
        isActive = false;
        if (callbackYes != null)
        {
            ADManager.Instance.ShowAds(WatchedAd, FailedToLoadAD, note);
        }
    }

    public void InvisBtnDown()
    {
        isInvisBtnDown = true;
    }
    
    public void InvisBtnUp()
    {
        isInvisBtnDown = false;
    }

    public void WatchedAd()
    {
        rect.gameObject.SetActive(false);
        rect2.gameObject.SetActive(true);
        rect2.anchoredPosition = new Vector2(0, 150);
        petAnim2.Init(PetInGameManager.Instance.pet.type);
    }

    public void ReadyButtonClicked()
    {
        bg.DOFade(0f, 0.5f);
        rect2.DOAnchorPosY(-500f, 0.4f).SetEase(Ease.OutExpo);
        rect2.DOPunchRotation(Vector3.one * 3f, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            callbackYes();
        });
    }

    public void FailedToLoadAD()
    {
        callbackNo();
        Hide();
    }
}
