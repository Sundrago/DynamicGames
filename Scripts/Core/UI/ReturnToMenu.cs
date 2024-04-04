using Core.System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.UI
{
    public class ReturnToMenu : MonoBehaviour
    {
        [FormerlySerializedAs("sfx")] [SerializeField]
        private SfxController sfxController;

        [FormerlySerializedAs("FromCanvas")] [SerializeField]
        private GameObject fromCanvas;

        [FormerlySerializedAs("MainCanvas")] [SerializeField]
        private GameObject mainCanvas;

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