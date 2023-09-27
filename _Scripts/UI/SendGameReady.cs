using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendGameReady : MonoBehaviour
{
    [SerializeField] GameObject eventHandler;
    [SerializeField] string type;

    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void GameReady()
    {
        switch(type)
        {
            case "build":
                eventHandler.GetComponent<BuildGameEventHandler>().StartGame();
                break;
            case "rocket":
                eventHandler.GetComponent<RocketPhysics>().StartGame();
                break;
            case "jump":
                eventHandler.GetComponent<Jump_StageCtrl>().BeginFirstGame();
                break;
        }
        
    }
}
