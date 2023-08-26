using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] SFXCTRL sfx;
    [SerializeField] GameObject FromCanvas, MainCanvas, TransitionCanvas;

    public void ReturnToMenuClkcked()
    {
        if(!TransitionCanvas.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("transition_idle")) return;

        sfx.PlaySfx(3);
        TransitionCanvas.GetComponent<transition_test>().canvas_A = FromCanvas;
        TransitionCanvas.GetComponent<transition_test>().canvas_B = MainCanvas;
        TransitionCanvas.GetComponent<transition_test>().ReturnToMenu = true;
        TransitionCanvas.GetComponent<Animator>().SetTrigger("start");
    }
}
