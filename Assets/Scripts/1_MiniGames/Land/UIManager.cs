using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Responsible for managing the user interface of the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private Image tutorialImageA, tutorialImageB;
        [SerializeField] private TextMeshProUGUI tutorialMessageText;
        [SerializeField] public TextMeshProUGUI successMessageText;

        public void InitializeTutorialPanel()
        {
            var transparent = new Color(1, 1, 1, 0);
            tutorialImageA.color = transparent;
            tutorialImageB.color = transparent;
            successMessageText.color = transparent;
            tutorialPanel.SetActive(false);
        }

        public void ShowTutorial()
        {
            if (tutorialPanel.activeSelf) return;

            tutorialPanel.SetActive(true);
            DOTween.Kill(tutorialImageA);
            DOTween.Kill(tutorialImageB);
            DOTween.Kill(tutorialMessageText);
            tutorialMessageText.DOFade(0.6f, 2f);
            tutorialImageA.DOFade(0.6f, 2f);
            tutorialImageB.DOFade(0.6f, 2f);
        }

        public void HideTutorial(float duration)
        {
            if (!tutorialPanel.activeSelf) return;

            DOTween.Kill(tutorialImageA);
            DOTween.Kill(tutorialImageB);
            DOTween.Kill(tutorialMessageText);
            tutorialMessageText.DOFade(0, duration);
            tutorialImageA.DOFade(0, duration);
            tutorialImageB.DOFade(0, duration)
                .OnComplete(() => { tutorialPanel.SetActive(false); });
        }

        public void DoFadeSuccessText(float endValue)
        {
            successMessageText.DOFade(endValue, 1f);
        }
    }
}