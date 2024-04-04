using DG.Tweening;
using DynamicGames.System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Manages the pet information panel.
    /// </summary>
    public class PetInfoPanelManager : MonoBehaviour
    {
        [Header("Constants")] private const float ScaleOffset = 1;

        private const float YPosOffsetScale = 1100;
        private const float YPosOffset = -90;
        private const float sliderSizeDeltaX = 910;

        [Header("UI Components")] [SerializeField]
        private Image previewImage;

        [SerializeField] private TextMeshProUGUI name_ui, level_ui, age_ui, skills_ui, from_ui, rank_ui;
        [SerializeField] private TextMeshProUGUI exp_ui;
        [SerializeField] private RectMask2D expSlider_ui;
        [SerializeField] private Transform panel;
        [SerializeField] private TextMeshProUGUI updateText;
        [SerializeField] private PetInfoPanelMover petInfoMover;
        private Transform petObj;
        private SpriteRenderer spriteRenderer;
        private PetType type;
        public static PetInfoPanelManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Time.frameCount % 5 == 0)
            {
                if (!IsRendererAvailable())
                    return;

                AdjustImageTransform();
            }
        }

        private bool IsRendererAvailable()
        {
            return spriteRenderer != null && !spriteRenderer.gameObject.activeSelf;
        }

        private void AdjustImageTransform()
        {
            if (previewImage.sprite != spriteRenderer.sprite)
                previewImage.sprite = spriteRenderer.sprite;
            previewImage.gameObject.transform.localScale =
                spriteRenderer.gameObject.transform.localScale * ScaleOffset;
            if (petObj.localScale.y * spriteRenderer.transform.localScale.x < 0)
                previewImage.gameObject.transform.localScale = new Vector3(
                    previewImage.gameObject.transform.localScale.x * -1,
                    previewImage.gameObject.transform.localScale.y,
                    1);
        }

        public void UpdatePetInfoUI(PetType type)
        {
            this.type = type;
            var petData = PetManager.Instance.GetPetDataByType(type);
            var petController = petData.obj.GetComponent<PetObject>();

            var petInfoMover = gameObject.GetComponent<PetInfoPanelMover>();
            if (petInfoMover != null)
                petInfoMover.targetPetPos =
                    petController.centerPoint;

            petObj = petData.obj.transform;
            spriteRenderer = petController.spriteRenderer;
            UpdatePreviewImage();

            if (PetManager.Instance.GetPetCount(type) == 0)
            {
                UpdateNoPet();
                return;
            }

            UpdatePetUIElements();
        }

        private void UpdatePreviewImage()
        {
            previewImage.sprite = spriteRenderer.sprite;
            previewImage.gameObject.transform.localScale = spriteRenderer.gameObject.transform.localScale * ScaleOffset;
            previewImage.gameObject.transform.localPosition = new Vector3(0,
                spriteRenderer.gameObject.transform.localPosition.y * YPosOffsetScale + YPosOffset, 0);
        }

        private void UpdateNoPet()
        {
            previewImage.color = Color.black;
            name_ui.text = "????";
            level_ui.text = "| ????";
            age_ui.text = "| ????";
            skills_ui.text = "????";
            from_ui.text = "| ????";
            rank_ui.text = "| ????";

            expSlider_ui.padding = new Vector4(0, 0, sliderSizeDeltaX, 0);
            exp_ui.text = "?";
        }

        private void UpdatePetUIElements()
        {
            previewImage.color = Color.white;

            var exp = PetManager.Instance.GetPetExp(type);
            var level = PetManager.Instance.GetPetLevel(type);

            name_ui.text = type.ToString();
            level_ui.text = "| Level : " + level;
            age_ui.text = "| Age : " + PetManager.Instance.GetPetAge(type);
            from_ui.text = "| From : " + PetDialogueManager.Instance.GetFrom(type);
            skills_ui.text = PetDialogueManager.Instance.GetDescr(type);
            rank_ui.text = "| Rank : " + PetDialogueManager.Instance.GetRank(type);

            var expNormal = exp / (level * 5f);
            expSlider_ui.padding = new Vector4(0, 0, sliderSizeDeltaX - sliderSizeDeltaX * expNormal, 0);
            exp_ui.text = exp + "/" + level * 5;
        }

        public void AttemptPetLevelUp()
        {
            if (PetManager.Instance.PetLevelUP(type))
            {
                UpdatePetInfoUI(type);
            }
            else
            {
                expSlider_ui.gameObject.transform.localPosition =
                    new Vector3(0, expSlider_ui.gameObject.transform.localPosition.y, 0);
                DOTween.Kill(expSlider_ui.gameObject.transform);
                expSlider_ui.gameObject.transform.DOPunchPosition(new Vector3(5, 0, 0), 1f, 5);
            }
        }

        [Button]
        public void ShowPanel(PetType _type)
        {
            UpdatePetInfoUI(_type);
            if (gameObject.activeSelf) return;

            InitiateShowAnimation();
        }

        private void InitiateShowAnimation()
        {
            panel.localPosition = new Vector3(0, -1500, 0);
            panel.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutExpo);
            gameObject.SetActive(true);
            updateText.gameObject.SetActive(false);
        }

        public void HidePanel(bool longTransition = false)
        {
            if (DOTween.IsTweening(panel) || gameObject.activeSelf == false)
                return;

            InitHideAnimation(longTransition);
        }

        private void InitHideAnimation(bool longTransition)
        {
            panel.DOLocalMoveY(-4000, longTransition ? 1.5f : 0.75f).SetEase(Ease.InOutExpo)
                .OnComplete(() =>
                {
                    TutorialManager.Instancee.FriendsPanelClosed();
                    gameObject.SetActive(false);
                    MainPage.MainPage.Instance.DeactivateAllBlocks();
                });
        }

        public void Hidden()
        {
            panel.localPosition = new Vector3(0, -1500, 0);
            gameObject.SetActive(false);
        }

        public void FeedBtnClicked()
        {
            updateText.color = Color.white;
            updateText.gameObject.SetActive(true);
            updateText.DOFade(0, 2f)
                .From();
        }
    }
}