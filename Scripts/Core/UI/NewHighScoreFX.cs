using Core.System;
using DG.Tweening;
using UnityEngine;

namespace Core.UI
{
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
            AudioManager.Instance.PlaySfxByTag(SfxTag.HighScore);
            gameObject.SetActive(true);

            DOTween.Kill(title);
            title.anchoredPosition = new Vector2(title.anchoredPosition.x, -350f);
            title.DOAnchorPosY(350, 1).SetEase(Ease.OutExpo);
            title.DOAnchorPosY(-500, 2).SetEase(Ease.InOutExpo).SetDelay(2)
                .OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}