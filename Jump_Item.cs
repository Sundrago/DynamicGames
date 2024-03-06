using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_Item : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(0, 100, 0); // Rotation speed in degrees per second


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
