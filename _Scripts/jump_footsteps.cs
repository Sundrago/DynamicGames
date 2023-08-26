using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_footsteps : MonoBehaviour
{
    public float y_speed;

    void Update()
    {
        gameObject.transform.Translate(0,y_speed*Time.deltaTime,0);
    }
}
