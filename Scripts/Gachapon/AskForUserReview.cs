using DG.Tweening;
using DynamicGames.System;
using DynamicGames.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

namespace DynamicGames.Gachapon
{
    /// <summary>
    ///     Responsible for managing the user review functionality.
    /// </summary>
    public class AskForUserReview : MonoBehaviour
    {
        [Header("UI Components")] [SerializeField]
        private Transform panel;

        [SerializeField] private GameObject msgbox_enjoying, msgbx_yes, msgbox_no;
        [SerializeField] private GameObject reviewCube;
        [SerializeField] private TextMeshProUGUI guideText;
        [SerializeField] private Image bgImage;
        private ReviewStatus status;

        private void Start()
        {
            status = (ReviewStatus)PlayerData.GetInt(DataKey.AskForUserReviewStatus);
            ChangeAndSaveStatus(status);
            gameObject.SetActive(false);

            if (status == ReviewStatus.NotRevealed)
            {
                reviewCube.SetActive(false);
            }
            else if (status == ReviewStatus.Revealed)
            {
                guideText.text = "Already Did! / Don't ask again";
                reviewCube.SetActive(true);
            }
            else if (status == ReviewStatus.NeverShowAgain)
            {
                Destroy(reviewCube);
                Destroy(gameObject);
            }
        }

        public void CubeFallAnimation()
        {
            if (status != ReviewStatus.NotRevealed) return;

            DOVirtual.DelayedCall(5f, () =>
            {
                AudioManager.Instance.PlaySfxByTag(SfxTag.CubeFall);
                ChangeAndSaveStatus(ReviewStatus.Revealed);
                reviewCube.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                reviewCube.SetActive(true);
            });
        }

        private void ChangeAndSaveStatus(ReviewStatus newStatus)
        {
            status = newStatus;
            PlayerData.SetInt(DataKey.AskForUserReviewStatus, (int)status);
        }

        public void ShowPanel()
        {
            if (gameObject.activeSelf) return;
            panel.transform.position = Vector3.zero;
            panel.transform.eulerAngles = Vector3.zero;
            bgImage.DOFade(0.2f, 1f);

            if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

            panel.transform.DOLocalMoveY(-2500f, 1f).From().SetEase(Ease.OutExpo);
            panel.transform.DORotate(new Vector3(0f, 0f, 50f), 0.75f).From().SetEase(Ease.OutBack);

            gameObject.SetActive(true);
            msgbox_enjoying.SetActive(true);
            msgbox_no.SetActive(false);
            msgbx_yes.SetActive(false);

            panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);
        }

        public void YesBtnClicked()
        {
            msgbox_enjoying.SetActive(false);
            msgbox_no.SetActive(false);
            msgbx_yes.SetActive(true);
            panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);
        }

        public void NoBtnClicked()
        {
            msgbox_enjoying.SetActive(false);
            msgbox_no.SetActive(true);
            msgbx_yes.SetActive(false);
            panel.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.5f);
        }

        public void LeaveReviewBtnClicked()
        {
#if UNITY_IOS
            Device.RequestStoreReview();
            Application.OpenURL(PrivateKey.AppstoreURL);
#endif
            HidePanel();
        }

        public void NeverShowAgainClicked()
        {
            ChangeAndSaveStatus(ReviewStatus.NeverShowAgain);
            HidePanel();
            DOVirtual.DelayedCall(2f, () =>
            {
                FXManager.Instance.CreateFX(FXType.RocketHit, reviewCube.transform.position);
                reviewCube.SetActive(false);
                Destroy(gameObject);
            });
        }

        public void HidePanel()
        {
            if (!gameObject.activeSelf || DOTween.IsTweening(panel.transform)) return;

            panel.transform.position = Vector3.zero;
            panel.transform.eulerAngles = Vector3.zero;
            bgImage.DOFade(0f, 1f);
            panel.transform.DORotate(new Vector3(0f, 0f, 100f), 0.75f).SetEase(Ease.OutExpo);
            panel.transform.DOLocalMoveY(-2500f, 1f).SetEase(Ease.OutExpo)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        private enum ReviewStatus
        {
            NotRevealed,
            Revealed,
            NeverShowAgain
        }
    }
}