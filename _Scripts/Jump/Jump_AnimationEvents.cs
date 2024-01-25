using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_AnimationEvents : MonoBehaviour
{
    [SerializeField] Jump_GameManager jumpStage;

    void Start()
    {
        gameObject.SetActive(false);
    }
    public void Preload(){
        jumpStage.ClearGame();
    }

    public void Ready() {
        jumpStage.BeginFirstGame();
    }
}
