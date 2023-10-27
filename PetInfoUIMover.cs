using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetInfoUIMover : MonoBehaviour
{
    [SerializeField]
    private Transform targetPetPos;
    [SerializeField]
    private Transform pointMarker;
    
    void Update()
    {
        if (targetPetPos != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targetPetPos.position);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, pos.y, 0);
            pointMarker.transform.position = new Vector3(pos.x, pointMarker.transform.position.y, 0);
        } 
    }
}
