using DynamicGames.System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DynamicGames.UI
{
    /// <summary>
    /// Handles returning to the menu.
    /// </summary>
    public class ReturnToMenu : MonoBehaviour
    {
        [SerializeField] private SfxController sfxController;
        [SerializeField] private GameObject fromCanvas;
        [SerializeField] private GameObject mainCanvas;

        public void OnReturnToMenuButtonClick()
        {
            if (!TransitionManager.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0)
                    .IsName("transition_idle")) return;

            sfxController.PlaySfx(3);
            TransitionManager.Instance.canvas_A = fromCanvas;
            TransitionManager.Instance.canvas_B = mainCanvas;
            TransitionManager.Instance.ReturnToMenu = true;
            TransitionManager.Instance.GetComponent<Animator>().SetTrigger("start");
        }
    }
}