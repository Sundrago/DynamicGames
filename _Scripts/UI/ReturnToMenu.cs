using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] SFXCTRL sfx;
    [SerializeField] private GameObject FromCanvas, MainCanvas;

    public void ReturnToMenuClkcked()
    {
        Debug.Log("ReturnToMenuClkcked");
        if(!TransitionManager.Instance.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("transition_idle")) return;

        sfx.PlaySfx(3);
        TransitionManager.Instance.canvas_A = FromCanvas;
        TransitionManager.Instance.canvas_B = MainCanvas;
        TransitionManager.Instance.ReturnToMenu = true;
        TransitionManager.Instance.GetComponent<Animator>().SetTrigger("start");
    }
}
