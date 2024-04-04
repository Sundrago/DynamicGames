using DG.Tweening;
using DynamicGames.MainPage;
using DynamicGames.MiniGames;
using DynamicGames.System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    public class SquareBlockCtrl : MonoBehaviour
    {
        public BlockDragHandler blockDragHandler;
        [SerializeField] private GameType gameType;
        [SerializeField] private Image mainImage, lockImage, white;
        [SerializeField] private Sprite lockedImage, unlockedImage;
        [SerializeField] private Transform shineObj;
        [SerializeField] public bool isNotGame;

        public Transform tl, tr, bl, br;
        public bool isLocked { get; set; }

        private void Start()
        {
            blockDragHandler = GetComponent<BlockDragHandler>();
        }

        public void Init()
        {
            if (isNotGame) return;
            if (mainImage == null || lockImage == null) return;

            if (isLocked)
            {
                if (white != null)
                    white.color = new Color(1f, 1f, 1f, 0f);
                mainImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                lockImage.color = new Color(1f, 1f, 1f, 0.5f);
                lockImage.sprite = lockedImage;
                lockImage.gameObject.SetActive(true);
            }
            else
            {
                mainImage.color = new Color(1f, 1f, 1f, 1f);
                lockImage.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        [Button]
        public void GetBoundaries()
        {
            var boundaries = GetChildWithName(gameObject.transform, "boundaries");
            tl = GetChildWithName(boundaries.transform, "tl").transform;
            tr = GetChildWithName(boundaries.transform, "tr").transform;
            bl = GetChildWithName(boundaries.transform, "bl").transform;
            br = GetChildWithName(boundaries.transform, "br").transform;

            GameObject GetChildWithName(Transform parent, string withName)
            {
                var amt = parent.childCount;
                for (var i = 0; i < amt; i++)
                {
                    var obj = parent.GetChild(i).gameObject;
                    if (obj.name == withName) return obj;
                }

                return null;
            }
        }
#endif

        public void Reveal()
        {
            if (isNotGame) return;
            DoShineFX(2f);
            DoReveal(2f);
            AudioManager.Instance.PlaySfxByTag(SfxTag.BlockReveal);
        }

        private void DoShineFX(float duration)
        {
            if (DOTween.IsTweening(shineObj)) DOTween.Kill(shineObj);

            shineObj.localPosition = new Vector3(-30, 0, 0);
            shineObj.DOLocalMoveX(30f, duration);
        }

        private void DoReveal(float duration)
        {
            if (DOTween.IsTweening(mainImage)) DOTween.Kill(mainImage);

            mainImage.color = new Color(1, 1, 1, 0);
            mainImage.DOFade(1, duration);
        }

        public void ShowLock()
        {
            if (isNotGame) return;
            if (!isLocked) return;
            LockDoFade(1);
        }

        public void HideLock()
        {
            if (isNotGame) return;
            if (!isLocked) return;
            LockDoFade(0.5f);
        }

        public void LockDoFade(float value)
        {
            if (isNotGame) return;
            lockImage.DOFade(value, 0.5f);
        }

        public void PunchLock()
        {
            if (isNotGame) return;
            if (!isLocked) return;
            lockImage.transform.localPosition = Vector3.zero;
            lockImage.transform.DOPunchPosition(new Vector3(0, 0.75f, 0), 1f, 7);
        }

        public void UnLock()
        {
            if (isNotGame) return;
            BlockStatusManager.Instance.SetBlockStatus(BlockStatusManager.Instance.GetBlockTypeByGameType(gameType),
                BlockStatusManager.BlockStatus.Unlocked);
            lockImage.transform.localPosition = Vector3.zero;
            lockImage.transform.DOShakePosition(0.5f, new Vector3(0.3f, 0.3f, 0));
            lockImage.DOFade(1, 0.5f);
            lockImage.gameObject.transform.DOScale(Vector3.one * 0.8f, 0.5f)
                .OnComplete(() =>
                {
                    lockImage.gameObject.transform.localScale = Vector3.one;
                    lockImage.sprite = unlockedImage;
                    lockImage.transform.localPosition = Vector3.zero;
                });
            mainImage.DOColor(Color.white, 0.5f).SetDelay(1f)
                .OnComplete(() => { DoShineFX(2f); });
            gameObject.transform.DOPunchScale(Vector3.one * 5, 0.7f, 5).SetDelay(0.5f);
            lockImage.DOFade(0f, 0.5f).SetDelay(0.5f);
            isLocked = false;
        }

        public void Hide()
        {
            white.gameObject.SetActive(true);
            white.DOFade(1f, 2f).SetEase(Ease.InCirc).SetDelay(1f);
            mainImage.DOFade(0, 0.2f).SetDelay(1.9f);
            lockImage.DOFade(0, 0.2f).SetDelay(1.9f);
            white.DOFade(0, 0.2f).SetDelay(1.9f);
        }
    }
}