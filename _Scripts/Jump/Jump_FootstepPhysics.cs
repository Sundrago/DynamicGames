using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_FootstepPhysics : MonoBehaviour
{
    public float y_speed;

    void Update()
    {
        gameObject.transform.Translate(0,y_speed*Time.deltaTime,0);
    }
}
