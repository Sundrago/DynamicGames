using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetInfoUIMover : MonoBehaviour
{
    [SerializeField]
    public Transform targetPetPos;
    [SerializeField]
    private Transform pointMarker, pointMarkerTop;

    [SerializeField] private float offsetY;

    [SerializeField] private Transform constraint_left, constraint_right;

    [SerializeField] private float lerpA, lerpB;
    [SerializeField] public Transform petSelectionIcon;
    [SerializeField] private Camera camera;

    private void Start()
    {
        offsetY = Screen.height / 7f + 150;
    }

    void Update()
    {
        if (targetPetPos != null)
        {
            // print(targetPetPos.position.y + " : " + targetPetPos.position.y/Screen.height);
            
            Vector3 pos = camera.WorldToScreenPoint(targetPetPos.position);
            Vector3 panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y + (targetPetPos.position.y < Screen.height/2f ? -offsetY : +offsetY), 0);
            // Vector3 pointTargetPos = new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x), pointMarker.transform.position.y, 0);
            

            if (targetPetPos.position.y < 0)
            {
                panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y + offsetY, 0);
                pointMarkerTop.gameObject.SetActive(false);
                pointMarker.gameObject.SetActive(true);
                pointMarker.transform.position = Vector3.Lerp(pointMarker.transform.position, new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x), pointMarker.transform.position.y, 0), lerpB);
                
            }
            else
            {
                panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y - (offsetY*1.2F), 0);
                pointMarkerTop.gameObject.SetActive(true);
                pointMarker.gameObject.SetActive(false);
                pointMarkerTop.transform.position = Vector3.Lerp(pointMarkerTop.transform.position, new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x), pointMarkerTop.transform.position.y, 0), lerpB);
            }
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, panelTargetPos, lerpA);
            petSelectionIcon.position = pos;
        } 
    }
}
