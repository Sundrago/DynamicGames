using System.Collections.Generic;
using AssetKits.ParticleImage;
using Core.Pet;
using Core.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.Gacha
{
    /// <summary>
    ///     Class responsible for playing gachapon capsule animations.
    /// </summary>
    public class CapsuleAnimation : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private GachaponManager gachaponManager;
        
        [FormerlySerializedAs("inventory")]
        [FormerlySerializedAs("petDrawer")]
        [Header("UI Components")] 
        [SerializeField] private PetInventory petInventory;
        [SerializeField] private Transform top, btm1, btm2, itemObj;
        [SerializeField] private Image item, item_white;
        [SerializeField] private List<Image> capsuleImages;
        [SerializeField] private List<ParticleImage> particleImages;
        [SerializeField] private GameObject isNewFx, isOldFx;
        [FormerlySerializedAs("newPetAnimationManager")] [FormerlySerializedAs("newPetAnim")] [SerializeField] private NewPetAnimManager newPetAnimManager;
        [SerializeField] private float sizeFactor, posFactor;
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
                case CapsuleStatus.ready:
                    PlayCapsuleOpenAnimation();
                    break;

                case CapsuleStatus.opened:
                    PlayCapsuleObtainAnimation();
                    break;

                case CapsuleStatus.inactive:
                    CapsuleAnimationFinished();
                    break;
            }
        }

        private enum CapsuleStatus
        {
            ready,
            opened,
            inactive
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
            var relativePosY = config.obj.GetComponent<Pet.PetObject>().spriteRenderer.gameObject.transform.localPosition.y *
                               posFactor;
            item.sprite = config.image;
            item.transform.localPosition = new Vector2(0, relativePosY);
            item.transform.localScale =
                config.obj.GetComponent<Pet.PetObject>().spriteRenderer.gameObject.transform.localScale.y * Vector3.one;
        }

        private void ResetPositions()
        {
            foreach (var img in capsuleImages)
                img.color = Color.white;

            itemObj.localScale = Vector3.one;
            itemObj.localPosition = Vector3.zero;
            gameObject.transform.localPosition = Vector3.zero;
            top.position = Vector3.zero;
            btm1.position = Vector3.zero;
            btm2.position = Vector3.zero;

            item.GetComponent<Mask>().showMaskGraphic = false;
            gameObject.transform.localEulerAngles = Vector3.zero;
            PauseParticles();
        }

        private void ResetColors()
        {
            item.color = Color.white;
            item_white.color = new Color(1, 1, 1, 0.3f);
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
            status = CapsuleStatus.ready;
            isAnimPlaying = false;
        }

        #endregion

        #region 2.Capsule Open Animation

        private void PlayCapsuleOpenAnimation()
        {
            StartOpenAnimation();
            PlayOpenAnimation();
            if (isNew) PlayNewPetAnimation();
        }

        private void StartOpenAnimation()
        {
            DOTween.Kill(gameObject.transform);
            gameObject.transform.localScale = Vector3.one;
            status = CapsuleStatus.opened;
            isAnimPlaying = true;

            StartParticles();
            AudioManager.Instance.PlaySfxByTag(SfxTag.GachaCapsuleOpen);
        }

        private void PlayOpenAnimation()
        {
            gameObject.transform.DOShakePosition(2, new Vector3(15, 30, 1), 7, 50);
            gameObject.transform.DOShakeRotation(1.5f, new Vector3(0, 0, 3), 10, 50);

            top.DOLocalMoveY(500, 1f).SetDelay(0.9f).SetEase(Ease.OutExpo);
            btm1.DOLocalMoveY(-500, 1f).SetEase(Ease.OutExpo).SetDelay(0.9f);
            btm2.DOLocalMoveY(-500, 1f).SetEase(Ease.OutExpo).SetDelay(0.9f);

            item_white.DOFade(0.7f, 0.135f)
                .SetLoops(6, LoopType.Yoyo)
                .SetEase(Ease.InOutCubic)
                .OnComplete(() =>
                {
                    item_white.DOFade(1, 0.2f)
                        .OnComplete(() =>
                        {
                            if (PetDialogueManager.Instance.GetRank(type) == 'S') newPetAnimManager.Init(type);
                            item.GetComponent<Mask>().showMaskGraphic = true;
                        });
                });
            item.transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 0.85f);
            item.transform.DOPunchScale(Vector3.one * 0.4f, 1f).SetDelay(0.9f)
                .OnComplete(() => { isAnimPlaying = false; });
            item_white.DOFade(0f, 1f).SetEase(Ease.OutSine).SetDelay(0.9f);
        }

        private void PlayNewPetAnimation()
        {
            DOVirtual.DelayedCall(0.15f, () => { AudioManager.Instance.PlaySfxByTag(SfxTag.GachaNewOpen); });
        }

        #endregion

        #region 3.Capsule Obtain Animation

        private void PlayCapsuleObtainAnimation()
        {
            status = CapsuleStatus.inactive;
            isAnimPlaying = true;
            PauseParticles();

            foreach (var img in capsuleImages) img.DOFade(0, 0.5f);

            targetItemPos = petInventory.GetItemTransformByType(type);
            midPos = Vector3.Lerp(itemObj.transform.position, targetItemPos.position, 0.5f);

            petInventory.ShowPanel(false, true);
            petInventory.SlideToItemByIdx(type);
            itemObj.DOMove(midPos, 0.4f)
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
                    itemObj.DOScale(0.2f, 0.2f);
                    itemObj.DOScale(0, 0.1f)
                        .SetDelay(0.2f)
                        .SetEase(Ease.InOutQuint);

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        if (isNew) AudioManager.Instance.PlaySfxByTag(SfxTag.GachaNewItem);
                        else AudioManager.Instance.PlaySfxByTag(SfxTag.GachaItem);
                        PetManager.Instance.AddPetCountByType(type);
                        petInventory.drawerItems[type].UpdateItemWithAnimation();
                    });
                });
        }

        private void CapsuleAnimationFinished()
        {
            petInventory.HidePanel();
            gachaponManager.CapsuleAnimFinished(isNew);
            if (isNew) PetManager.Instance.InstantiateNewPetFX(type);
            gameObject.SetActive(false);
        }

        private void MoveItemObj(float _normal)
        {
            itemObj.position = Vector3.Lerp(midPos, targetItemPos.position, _normal);
        }

        #endregion
    }
}