using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land_RocketCollisionHandler : MonoBehaviour
{
    [SerializeField] Land_GameManager rocket;
    [SerializeField] string idx;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (idx == "island") return;
        rocket.ChangeColliderState(idx, true);
        rocket.failPosition = col.contacts[0].point;
        rocket.failRotation = col.transform.rotation;
    }
    void OnCollisionExit2D(Collision2D col)
    {
        if (idx == "island") return;
        rocket.ChangeColliderState(idx, false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (idx == "island")
        {
            switch(col.gameObject.name)
            {
                case "landing_left":
                    rocket.ChangeColliderState("left", true);
                    break;
                case "landing_right":
                    rocket.ChangeColliderState("right", true);
                    break;
            }
        }
    }
}
