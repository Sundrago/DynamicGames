using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_anim_events : MonoBehaviour
{
    [SerializeField] Jump_StageCtrl jumpStage;

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
