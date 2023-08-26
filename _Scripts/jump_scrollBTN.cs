using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class jump_scrollBTN : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject cylindar;

    [SerializeField] public Vector3 targetRotation;
    [SerializeField] float updateSpeed;
    [SerializeField] float scrollSpeed;

    void Update()
    {
        if(Vector3.Distance(cylindar.transform.localEulerAngles, targetRotation) > 0.05f) {
            Vector3 newAngle = Vector3.Lerp(cylindar.transform.localEulerAngles, targetRotation, updateSpeed);
            cylindar.transform.localEulerAngles = newAngle;
        }
    }

    public void OnPointerMove(PointerEventData eventData) 
	{
        float diff = (eventData.delta.x);
        Vector3 originalAngle = cylindar.transform.localEulerAngles;
        originalAngle.y -= diff * scrollSpeed;
        targetRotation = originalAngle;
	}
    
    public void OnPointerExit(PointerEventData eventData) {
        targetRotation = cylindar.transform.localEulerAngles;
    }
}
