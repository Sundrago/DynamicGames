using System;
using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Utility;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DynamicGames.System
{
    /// <summary>
    /// Handles the display and behavior of popup text messages.
    /// </summary>
    public class PopupTextManager : MonoBehaviour
    {
        [Header("UI Elements")] 
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Transform panelTransform;
        [SerializeField] private Button okayButton, yesButton, noButton;
        [SerializeField] private TextMeshProUGUI okayText, yesText, noText, messageText;
        [SerializeField] private TypewriterByCharacter typeWriter;
        [SerializeField] private List<GameObject> fluffyAnimations;

        private Action callbackOK, callbackYES, callbackNO;

        public static PopupTextManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowYesNoPopup(string message, Action yesFunction = null, Action noFunction = null,
            string yesButtonText = "[DEFAULT_YES]", string noButtonText = "[DEFAULT_NO]")
        {
            PreparePopup(message, yesButtonText, noButtonText);
            callbackYES = yesFunction;
            callbackNO = noFunction;
            ToggleButtons(false, true, true);
        }

        public void ShowOKPopup(string message, Action okFunction = null, string okButtonText = "[DEFAULT_OKAY]")
        {
            PreparePopup(message, okButtonText);
            callbackOK = okFunction;
            ToggleButtons(true, false, false);
        }

        private void PreparePopup(string message, string buttonTextYes = "", string buttonTextNo = "")
        {
            messageText.text = " " + Localize.GetLocalizedString(message);
            okayText.text = Localize.GetLocalizedString(buttonTextYes);
            yesText.text = Localize.GetLocalizedString(buttonTextYes);
            noText.text =
                Localize.GetLocalizedString(buttonTextNo); // Assuming this is not a mistake since no use case provided.

            ShowPanel();
            typeWriter.ShowText(" " + messageText.text);
            typeWriter.StartShowingText();
        }

        private void ToggleButtons(bool showOkay, bool showYes, bool showNo)
        {
            okayButton.gameObject.SetActive(showOkay);
            yesButton.gameObject.SetActive(showYes);
            noButton.gameObject.SetActive(showNo);

            okayButton.interactable = false;
            yesButton.interactable = false;
            noButton.interactable = false;
        }

        public void OnTypeWriterFinished()
        {
            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Open);
            Debug.Log("OnTypeWriterFinished");

            okayButton.interactable = true;
            yesButton.interactable = true;
            noButton.interactable = true;
        }

        public void HidePanel()
        {
            if (!gameObject.activeSelf) return;
            if (DOTween.IsTweening(panelTransform)) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Close);
            var hideSequence = DOTween.Sequence();
            hideSequence.Append(backgroundImage.DOFade(0, 0.25f));
            hideSequence.Join(panelTransform.DOScale(0.8f, 0.25f).SetEase(Ease.OutExpo));
            hideSequence.Join(panelTransform.DOShakePosition(0.3f, new Vector3(10, 10, 0)).SetEase(Ease.OutQuad));
            hideSequence.Append(panelTransform.DOLocalMoveY(3000, 0.7f).SetDelay(0.15f).SetEase(Ease.InOutExpo));
            hideSequence.OnComplete(() => gameObject.SetActive(false));
        }

        public void ShowPanel()
        {
            if (DOTween.IsTweening(panelTransform)) DOTween.Kill(panelTransform);
            if (DOTween.IsTweening(backgroundImage)) DOTween.Kill(backgroundImage);

            panelTransform.localScale = Vector3.one * 0.7f;
            panelTransform.localPosition = Vector3.zero;
            panelTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutElastic);

            gameObject.SetActive(true);
            backgroundImage.DOFade(0.3f, 0.5f);

            RandomFluffyAnimation();
        }

        private void RandomFluffyAnimation()
        {
            var rnd = Random.Range(0, fluffyAnimations.Count);
            for (var i = 0; i < fluffyAnimations.Count; i++) fluffyAnimations[i].SetActive(i == rnd);
        }

        public void OnButtonClick(int buttonIndex)
        {
            if (DOTween.IsTweening(panelTransform)) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Select);
            ToggleButtons(false, false, false); // Disable all buttons

            switch (buttonIndex)
            {
                case 0:
                    callbackOK?.Invoke();
                    break;
                case 1:
                    callbackYES?.Invoke();
                    break;
                case 2:
                    callbackNO?.Invoke();
                    break;
            }

            HidePanel();
        }
    }
}