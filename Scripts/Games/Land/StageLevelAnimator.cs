using Core.System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Land
{
    /// <summary>
    /// Controls the animation of the stage in the game.
    /// </summary>
    public class StageLevelAnimator : MonoBehaviour
    {
        [FormerlySerializedAs("gameManagerManager")] [SerializeField] private GameManager gameManager;
        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI stage_lv_text;
        [SerializeField] public int currentStageLevel = 1;


        public void SetLevel(int level)
        {
            currentStageLevel = level;
            if (currentStageLevel <= 9) stage_lv_text.text = '0' + currentStageLevel.ToString();
            else stage_lv_text.text = currentStageLevel.ToString();
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