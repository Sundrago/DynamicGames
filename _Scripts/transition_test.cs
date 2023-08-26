using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transition_test : MonoBehaviour
{
    public GameObject canvas_A, canvas_B, canvas_transition, rocket, build, jump;
    public bool ReturnToMenu = false;
    public bool OnTransition = false;

    public void StartTransition()
    {
        if(OnTransition) return;
        gameObject.GetComponent<Animator>().SetTrigger("play");
        OnTransition = true;
    }

    public void OnTransitionPoint()
    {
        if(ReturnToMenu)
        {
            if(rocket.activeInHierarchy) rocket.GetComponent<RocketPhysics>().ClearGame();
            if(build.activeInHierarchy) build.GetComponent<BuildGameEventHandler>().ClearGame();
            if(jump.activeInHierarchy) jump.GetComponent<Jump_StageCtrl>().ClearGame();

            canvas_A.SetActive(false);
            canvas_B.SetActive(true);
            ReturnToMenu = false;
            OnTransition = false;

            if(canvas_B.GetComponent<MainCanvas>() != null) {
            canvas_B.GetComponent<MainCanvas>().WentBackHome();
            }

            return;
        }
        canvas_transition.GetComponent<Animator>().Play("transition_out");
    }

    public void HideCanvasA()
    {
        OnTransition = false;
        canvas_B.SetActive(true);
        if(canvas_B.GetComponent<Animator>() != null) canvas_B.GetComponent<Animator>().SetTrigger("play");
        canvas_A.SetActive(false);
    }
}
