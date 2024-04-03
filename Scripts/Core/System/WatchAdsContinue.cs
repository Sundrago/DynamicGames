using System;
using Core.Pet;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.System
{
    public class WatchAdsContinue : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private RectMask2D rectMask2D;
        [SerializeField] private Button yesBtn;
        [SerializeField] private RectTransform initialRect;
        [SerializeField] private RectTransform finalRect;
        [SerializeField] private UIPetSpriteAnimator petAnim, petAnim2;
        [SerializeField] private Image adBackground;
     
        private const float duration = 3f;

        private Action callbackYes, callbackNo;
        private string adDescription;
        private bool isActive, isHiddenButtonPressed;
        private float startTime;

        public static WatchAdsContinue Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isActive) return;

            if (isHiddenButtonPressed) startTime -= Time.deltaTime * 1.5f;

            if (Time.time > startTime + duration)
            {
                callbackNo?.Invoke();
                isActive = false;
                Hide();
                yesBtn.interactable = false;
                return;
            }

            var normal = (duration - Time.time + startTime) / duration;
            UpdateProgressBar(normal);
        }

        private void UpdateProgressBar(float normal)
        {
            normal = normal * normal;
            rectMask2D.padding =
                new Vector4(0, 0, rectMask2D.GetComponent<RectTransform>().sizeDelta.x * (1 - normal), 0);
        }

        public void Init(Action callbackYes, Action callbackNo, string note)
        {
            adDescription = note;
            this.callbackYes = callbackYes;
            this.callbackNo = callbackNo;

            if (!PetInGameManager.Instance.EnterGameWithPet)
            {
                callbackNo?.Invoke();
                return;
            }

            InitActiveState();
            InitUIComponents();
            InitBgAndPetMotion();
        }

        private void InitActiveState()
        {
            isActive = false;
            yesBtn.interactable = false;
            UpdateProgressBar(1);
            isHiddenButtonPressed = false;
        }

        private void InitUIComponents()
        {
            DOTween.Kill(initialRect);
            initialRect.anchoredPosition = new Vector2(0f, -500f);
            initialRect.DOAnchorPosY(150, 0.5f).SetEase(Ease.OutExpo);
            initialRect.DOPunchRotation(Vector3.one * 3f, 0.55f).OnComplete(() =>
            {
                isActive = true;
                yesBtn.interactable = true;
                startTime = Time.time;
            });
        }

        private void InitBgAndPetMotion()
        {
            petAnim.Init(PetInGameManager.Instance.petController.type);
            gameObject.SetActive(true);

            adBackground.color = new Color(0, 0, 0, 0);
            adBackground.DOFade(0.3f, 1f);

            initialRect.gameObject.SetActive(true);
            finalRect.gameObject.SetActive(false);
        }


        private void Hide()
        {
            DOTween.Kill(initialRect);
            adBackground.DOFade(0f, 0.5f);
            initialRect.DOAnchorPosY(-500f, 0.4f).SetEase(Ease.OutExpo);
            initialRect.DOPunchRotation(Vector3.one * 3f, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }

        public void YesBtnClicked()
        {
            DOTween.Kill(initialRect);
            yesBtn.interactable = false;
            isActive = false;
            if (callbackYes != null) ADManager.Instance.ShowAds(WatchedAd, FailedToLoadAD, adDescription);
        }

        public void InvisBtnDown()
        {
            isHiddenButtonPressed = true;
        }

        public void InvisBtnUp()
        {
            isHiddenButtonPressed = false;
        }

        public void WatchedAd()
        {
            initialRect.gameObject.SetActive(false);
            finalRect.gameObject.SetActive(true);
            finalRect.anchoredPosition = new Vector2(0, 150);
            petAnim2.Init(PetInGameManager.Instance.petController.type);
        }

        public void ReadyButtonClicked()
        {
            adBackground.DOFade(0f, 0.5f);
            finalRect.DOAnchorPosY(-500f, 0.4f).SetEase(Ease.OutExpo);
            finalRect.DOPunchRotation(Vector3.one * 3f, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                callbackYes?.Invoke();
            });
        }

        public void FailedToLoadAD()
        {
            callbackNo?.Invoke();
            Hide();
        }
    }
}