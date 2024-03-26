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
                eventHandler.GetComponent<Games.Build.GameManager>().OnGameEnter();
                break;
            case "rocket":
                eventHandler.GetComponent<Land_GameManager>().StartGame();
                break;
            case "jump":
                eventHandler.GetComponent<Games.Jump.GameManager>().BeginFirstGame();
                break;
        }
        
    }
}
