using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAnimEnd : MonoBehaviour
{
    [SerializeField] RocketPhysics rocket;
    public string idx;

    public void AnimEndEvent()
    {
        switch(idx)
        {
            case "rocket":
                rocket.RocketReady();
                break;
            case "clear":
                rocket.ResetRocket();
                break;
        }
    }

    public void AnimEndEvent2()
    {
        switch (idx)
        {
            case "rocket":
                rocket.OpenNextStage();
                break;
        }
    }
}
