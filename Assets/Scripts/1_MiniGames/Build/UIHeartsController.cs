using DG.Tweening;
using UnityEngine;

namespace DynamicGames.MiniGames.Build
{
    /// <summary>
    ///     A controller for managing the UI and logic of the hearts in the build game.
    /// </summary>
    public class UIHeartsController : MonoBehaviour
    {
        [SerializeField] private UIHeartsElement[] hearts = new UIHeartsElement[5];

        private void Start()
        {
            foreach (var heart in hearts) SetAlphaAnim(heart, false);

            gameObject.SetActive(true);
        }

        /// <summary>
        ///     Sets the number of filled hearts in the UI representation.
        /// </summary>
        /// <param name="amount">The number of filled hearts.</param>
        public void SetHearts(int amount)
        {
            for (var i = 0; i < hearts.Length; i++) hearts[i].IsFilled = i < amount;
        }

        /// <summary>
        ///     Shows or hides the UI representation of the hearts.
        /// </summary>
        /// <param name="show">If set to true, the hearts will be shown. If set to false, the hearts will be hidden.</param>
        public void Show(bool show)
        {
            foreach (var heart in hearts) SetAlphaAnim(heart, show);

            gameObject.SetActive(true);
        }

        private void SetAlphaAnim(UIHeartsElement heart, bool show)
        {
            var endValue = show ? 1f : 0f;
            var duration = show ? 0.15f : 1.5f;

            DOTween.Kill(heart.heartFill);
            heart.heartFill.DOFade(endValue, duration);
            DOTween.Kill(heart.heartFrame);
            heart.heartFrame.DOFade(endValue, duration);
        }
    }
}