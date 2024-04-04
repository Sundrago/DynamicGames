using DynamicGames.System;
using TMPro;
using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Controls the animation of the stage in the game.
    /// </summary>
    public class StageLevelAnimator : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI stageLevelText;
        [SerializeField] public int currentStageLevel = 1;

        public void SetLevel(int level)
        {
            currentStageLevel = level;
            if (currentStageLevel <= 9) stageLevelText.text = '0' + currentStageLevel.ToString();
            else stageLevelText.text = currentStageLevel.ToString();
        }

        public void PlayAnim()
        {
            gameObject.GetComponent<Animator>().SetTrigger("show");
            AudioManager.Instance.PlaySfxByTag(SfxTag.RocketNewLevel);
        }

        public void HideAnim()
        {
            gameObject.GetComponent<Animator>().SetTrigger("hide");
        }

        public void NextLevel()
        {
            SetLevel(currentStageLevel + 1);
        }
    }
}