using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetInfoUIMover : MonoBehaviour
{
    [SerializeField]
    public Transform targetPetPos;
    [SerializeField]
    private Transform pointMarker;

    [SerializeField] private float offsetY;

    [SerializeField] private Transform constraint_left, constraint_right;

    [SerializeField] private float lerpA, lerpB;
    [SerializeField] public Transform petSelectionIcon;
    void Update()
    {
        if (targetPetPos != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targetPetPos.position);
            Vector3 panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y + offsetY, 0);
            Vector3 pointTargetPos = new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x), pointMarker.transform.position.y, 0);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, panelTargetPos, lerpA);
            pointMarker.transform.position = Vector3.Lerp(pointMarker.transform.position, new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x), pointMarker.transform.position.y, 0), lerpB);
            petSelectionIcon.position = pos;
        } 
    }
}
