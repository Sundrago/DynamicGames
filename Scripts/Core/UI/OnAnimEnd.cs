using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Land
{
    

public class OnAnimEnd : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public string idx;

    public void AnimEndEvent()
    {
        switch(idx)
        {
            case "rocket":
                gameManager.RocketIsReady();
                break;
            case "clear":
                gameManager.ResetRocket();
                break;
        }
    }

    public void AnimEndEvent2()
    {
        switch (idx)
        {
            case "rocket":
                gameManager.OpenNextStage();
                break;
        }
    }
}
}