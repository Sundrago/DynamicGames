using System;
using System.Collections;
using System.Collections.Generic;
using Core.Main;
using UnityEngine;
using Games;

public class TransitionManager : MonoBehaviour
{
    public GameObject canvas_A, canvas_B, canvas_transition, rocket, build, jump, shoot;
    public bool ReturnToMenu = false;
    public bool OnTransition = false;

    public static TransitionManager Instance;
    
    [SerializeField]
    private GameObject adj;

    private void Awake()
    {
        Instance = this;
    }

    public void StartTransition()
    {
        if(OnTransition) return;
        gameObject.GetComponent<Animator>().SetTrigger("play");
        OnTransition = true;
    }

    public void OnTransitionPoint()
    {
        adj.gameObject.SetActive(true);
        if(ReturnToMenu)
        {
            if(rocket.activeInHierarchy) rocket.GetComponent<Games.Land.GameManager>().ClearGame();
            if(build.activeInHierarchy) build.GetComponent<Games.Build.GameManager>().ClearGame();
            if(jump.activeInHierarchy) jump.GetComponent<Games.Jump.GameManager>().ClearGame();
            if(shoot.activeInHierarchy) shoot.GetComponent<Games.Shoot.GameManager>().ClearGame();

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
