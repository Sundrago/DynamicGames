using System.Text.RegularExpressions;
using DG.Tweening;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Controls the dialogue behavior of a pet.
    /// </summary>
    public class PetDialogueController : MonoBehaviour
    {
        [Header("UI Components")] 
        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private TextMeshProUGUI outline_ui, main_ui;
        [SerializeField] private Image bgImage, topTail, btmTail;

        [Header("Game Components")] 
        [SerializeField] private RectTransform rect;
        [SerializeField] private Transform dialogueLeft, dialogueRight, canvasLeft, canvasRight, tails;
        [SerializeField] private float diff;

        public float offsetY;
        private float endTime;
        private DialogueStatus status = DialogueStatus.Hidden;
        private Transform targetTransform;

        private void Update()
        {
            HandleDialogueStatus();
            RepositionDialogue();
            UpdateTails();
        }

        private void HandleDialogueStatus()
        {
            if (status == DialogueStatus.Hidden)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.transform.position = targetTransform.position;
            gameObject.transform.Translate(new Vector3(0, offsetY, 0));
            if (status == DialogueStatus.Appear)
                if (Time.time > endTime)
                    Hide();
        }

        private void RepositionDialogue()
        {
            if (dialogueLeft.transform.position.x < canvasLeft.transform.position.x + diff)
                gameObject.transform.Translate(
                    canvasLeft.transform.position.x + diff - dialogueLeft.transform.position.x, 0, 0);
            else if (dialogueRight.transform.position.x > canvasRight.transform.position.x - diff)
                gameObject.transform.Translate(
                    -(dialogueRight.transform.position.x - (canvasRight.transform.position.x - diff)), 0, 0);
        }

        private void UpdateTails()
        {
            if (targetTransform != null)
                tails.transform.position = new Vector3(targetTransform.position.x, tails.transform.position.y, 0);
        }

        public void Init(string input, Transform targetTransform, bool forceShow = false, float offsetY = 0.4f,
            float duration = 5)
        {
            if (status != DialogueStatus.Hidden && !forceShow) return;
            if (!targetTransform.gameObject.activeSelf) return;

            this.offsetY = offsetY;
            this.targetTransform = targetTransform;

            ResetUIElements();
            HandleTailPosition(offsetY);
            SetFontAndFade(input, duration);

            status = DialogueStatus.Appear;
            rect.SetAsLastSibling();
        }

        private void ResetUIElements()
        {
            DOTween.Kill(main_ui);
            DOTween.Kill(bgImage);
        }

        private void HandleTailPosition(float offsetY)
        {
            if (offsetY > 0)
            {
                topTail.gameObject.SetActive(false);
                btmTail.gameObject.SetActive(true);
                DOTween.Kill(btmTail);
                btmTail.DOFade(0.8f, 0.3f);
            }
            else
            {
                topTail.gameObject.SetActive(true);
                btmTail.gameObject.SetActive(false);
                DOTween.Kill(topTail);
                topTail.DOFade(0.8f, 0.3f);
            }
        }

        private void SetFontAndFade(string input, float duration)
        {
            outline_ui.color = Color.clear;
            outline_ui.text = Regex.Replace(input, "<.*?>", string.Empty);
            endTime = Time.time + duration;

            bgImage.color = new Color(0, 0, 0, 0);
            bgImage.DOFade(0.8f, 0.5f);
            main_ui.color = new Color(1, 1, 1, 0);
            main_ui.DOFade(0.8f, 0.3f);

            gameObject.SetActive(true);
            typewriter.ShowText(input);
            typewriter.StartShowingText(true);
        }

        public void Hide()
        {
            if (status != DialogueStatus.Appear) return;

            KillAnimationTween();
            status = DialogueStatus.Disappear;
            typewriter.StartDisappearingText();
            InitiateFadeAnimation();
        }

        private void KillAnimationTween()
        {
            DOTween.Kill(main_ui);
            DOTween.Kill(bgImage);
            DOTween.Kill(topTail);
            DOTween.Kill(btmTail);
        }

        private void InitiateFadeAnimation()
        {
            bgImage.DOFade(0, 2f).OnComplete(
                () =>
                {
                    status = DialogueStatus.Hidden;
                    gameObject.SetActive(false);
                });
            main_ui.DOFade(0, 2.5f);
            topTail.DOFade(0, 2.5f);
            btmTail.DOFade(0, 2.5f);
        }

        private enum DialogueStatus
        {
            Appear,
            Disappear,
            Hidden
        }
    }
}