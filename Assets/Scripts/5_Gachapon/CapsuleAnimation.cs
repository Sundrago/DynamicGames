using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using DynamicGames.Pet;
using DynamicGames.System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.Gachapon
{
    /// <summary>
    ///     Responsible for playing gachapon capsule animations.
    /// </summary>
    public class CapsuleAnimation : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private GachaponManager gachaponManager;
        [SerializeField] private NewPetAnimManager newPetAnimManager;

        [Header("UI Components")] 
        [SerializeField] private PetInventoryUIManager petInventoryUIManager;
        [SerializeField] private Transform topCapsule, btmCapsule1, btmCapsule2, itemObject;
        [SerializeField] private Image rewardItem, rewardItemMask;
        [SerializeField] private List<Image> capsuleImages;
        [SerializeField] private List<ParticleImage> particleImages;
        [SerializeField] private GameObject isNewFx, isOldFx;
        
        private const float SizeFactor = 1;
        private const float PositionFactor = 2200;
     
        private bool isNew, isAnimPlaying;
        private Vector3 midPos;
        private List<float> particleRateOverTimes;
        private CapsuleStatus status;
        private Transform targetItemPos;
        private PetType type;

        private void Start()
        {
            InitializeParticleRateOverTimes();
            gameObject.SetActive(false);
        }

        private void InitializeParticleRateOverTimes()
        {
            particleRateOverTimes = new List<float>();
            foreach (var particle in particleImages)
            {
                particleRateOverTimes.Add(particle.rateOverTime);
                particle.gameObject.SetActive(false);
            }
        }


        private void StartParticles()
        {
            for (var i = 0; i < particleImages.Count; i++)
            {
                particleImages[i].rateOverTime = particleRateOverTimes[i];
                particleImages[i].gameObject.SetActive(true);
            }

            isNewFx.SetActive(isNew);
            isOldFx.SetActive(!isNew);
        }

        private void PauseParticles()
        {
            foreach (var particleImage in particleImages)
            {
                particleImage.rateOverTime = 0;
                particleImage.gameObject.SetActive(true);
            }
        }

        /// <summary>
        ///     Plays gachapon capsule animations.
        /// </summary>
        public void BtnClicked()
        {
            if (isAnimPlaying) return;

            switch (status)
            {
                case CapsuleStatus.Ready:
                    PlayCapsuleOpenAnimation();
                    break;

                case CapsuleStatus.Opened:
                    PlayCapsuleObtainAnimation();
                    break;

                case CapsuleStatus.Inactive:
                    CapsuleAnimationFinished();
                    break;
            }
        }

        private enum CapsuleStatus
        {
            Ready,
            Opened,
            Inactive
        }

        #region 1.Ready Animation

        /// <summary>
        ///     Play animation to get ready for gachapon capsule opening.
        /// </summary>
        public void PlayReadyAnimation()
        {
            isAnimPlaying = true;

            ResetItemState();
            ResetPositions();
            ResetColors();
            CapsuleFrameInAnimation();
        }

        private void ResetItemState()
        {
            type = SelectPetType();
            var data = PetManager.Instance.GetPetDataByType(type);
            isNew = PetManager.Instance.GetPetCount(type) == 0;
            UpdatePetTransform(data);
            gameObject.SetActive(true);
        }

        private PetType SelectPetType()
        {
            if (PetManager.Instance.GetTotalPetCount() == 0 ||
                PetManager.Instance.GetPetCount(PetType.Fluffy) == 0)
                return PetType.Fluffy;
            return PetManager.Instance.GetRandomPetConfig().type;
        }

        private void UpdatePetTransform(PetConfig config)
        {
            var relativePosY =
                config.obj.GetComponent<PetObject>().spriteRenderer.gameObject.transform.localPosition.y *
                PositionFactor;
            rewardItem.sprite = config.image;
            rewardItem.transform.localPosition = new Vector2(0, relativePosY);
            rewardItem.transform.localScale =
                config.obj.GetComponent<PetObject>().spriteRenderer.gameObject.transform.localScale.y * Vector3.one;
        }

        private void ResetPositions()
        {
            foreach (var img in capsuleImages)
                img.color = Color.white;

            itemObject.localScale = Vector3.one;
            itemObject.localPosition = Vector3.zero;
            gameObject.transform.localPosition = Vector3.zero;
            topCapsule.position = Vector3.zero;
            btmCapsule1.position = Vector3.zero;
            btmCapsule2.position = Vector3.zero;

            rewardItem.GetComponent<Mask>().showMaskGraphic = false;
            gameObject.transform.localEulerAngles = Vector3.zero;
            PauseParticles();
        }

        private void ResetColors()
        {
            rewardItem.color = Color.white;
            rewardItemMask.color = new Color(1, 1, 1, 0.3f);
        }

        private void CapsuleFrameInAnimation()
        {
            gameObject.transform.DOLocalMoveY(-2000f, 1f)
                .SetEase(Ease.OutBack)
                .From();
            gameObject.transform.DOLocalRotate(new Vector3(0, 0, -30f), 1f)
                .From()
                .OnComplete(() => OnReadyAnimComplete());
        }

        private void OnReadyAnimComplete()
        {
            gameObject.transform.DOScale(1.05f, 0.2f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo);
            status = CapsuleStatus.Ready;
            isAnimPlaying = false;
        }

        #endregion

        #region 2.Capsule Open Animation

        private void PlayCapsuleOpenAnimation()
        {
            StartOpenAnimation();
            PlayOpenAnimation();
            if (isNew) PlayNewPetAnimationSound();
        }

        private void StartOpenAnimation()
        {
            DOTween.Kill(gameObject.transform);
            gameObject.transform.localScale = Vector3.one;
            status = CapsuleStatus.Opened;
            isAnimPlaying = true;

            StartParticles();
            AudioManager.Instance.PlaySfxByTag(SfxTag.GachaCapsuleOpen);
        }

        private void PlayOpenAnimation()
        {
            gameObject.transform.DOShakePosition(2, new Vector3(15, 30, 1), 7, 50);
            gameObject.transform.DOShakeRotation(1.5f, new Vector3(0, 0, 3), 10, 50);

            topCapsule.DOLocalMoveY(500, 1f).SetDelay(0.9f).SetEase(Ease.OutExpo);
            btmCapsule1.DOLocalMoveY(-500, 1f).SetEase(Ease.OutExpo).SetDelay(0.9f);
            btmCapsule2.DOLocalMoveY(-500, 1f).SetEase(Ease.OutExpo).SetDelay(0.9f);

            rewardItemMask.DOFade(0.7f, 0.135f)
                .SetLoops(6, LoopType.Yoyo)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    rewardItemMask.DOFade(1, 0.2f)
                        .OnComplete(() =>
                        {
                            if (PetDialogueManager.Instance.GetRank(type) == 'S') newPetAnimManager.Init(type);
                            rewardItem.GetComponent<Mask>().showMaskGraphic = true;
                        });
                });
            rewardItem.transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 0.85f);
            rewardItem.transform.DOPunchScale(Vector3.one * 0.4f, 1f).SetDelay(0.9f)
                .OnComplete(() => { isAnimPlaying = false; });
            rewardItemMask.DOFade(0f, 1f).SetEase(Ease.OutSine).SetDelay(0.9f);
        }

        private void PlayNewPetAnimationSound()
        {
            DOVirtual.DelayedCall(0.15f, () => { AudioManager.Instance.PlaySfxByTag(SfxTag.GachaNewOpen); });
        }

        #endregion

        #region 3.Capsule Obtain Animation

        private void PlayCapsuleObtainAnimation()
        {
            status = CapsuleStatus.Inactive;
            isAnimPlaying = true;
            PauseParticles();

            foreach (var img in capsuleImages) img.DOFade(0, 0.5f);

            targetItemPos = petInventoryUIManager.GetItemTransformByType(type);
            midPos = Vector3.Lerp(itemObject.transform.position, targetItemPos.position, 0.5f);

            petInventoryUIManager.ShowPanel(false, true);
            petInventoryUIManager.SlideToItemByIdx(type);
            itemObject.DOMove(midPos, 0.4f)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    DOVirtual.Float(0f, 1f, 0.4f, MoveItemObj)
                        .SetEase(Ease.OutQuart);
                    targetItemPos.localScale = Vector3.one;
                    targetItemPos.DOPunchScale(Vector2.one * 0.5f, 1f)
                        .SetDelay(0.2f)
                        .OnComplete(() =>
                        {
                            isAnimPlaying = false;
                            BtnClicked();
                        });
                    itemObject.DOScale(0.2f, 0.2f);
                    itemObject.DOScale(0, 0.1f)
                        .SetDelay(0.2f)
                        .SetEase(Ease.InOutQuint);

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        if (isNew) AudioManager.Instance.PlaySfxByTag(SfxTag.GachaNewItem);
                        else AudioManager.Instance.PlaySfxByTag(SfxTag.GachaItem);
                        PetManager.Instance.AddPetCountByType(type);
                        petInventoryUIManager.drawerItems[type].UpdateItemWithAnimation();
                    });
                });
        }

        private void CapsuleAnimationFinished()
        {
            petInventoryUIManager.HidePanel();
            gachaponManager.CapsuleAnimFinished(isNew);
            if (isNew) PetManager.Instance.InstantiateNewPetFX(type);
            gameObject.SetActive(false);
        }

        private void MoveItemObj(float normal)
        {
            itemObject.position = Vector3.Lerp(midPos, targetItemPos.position, normal);
        }

        #endregion
    }
}