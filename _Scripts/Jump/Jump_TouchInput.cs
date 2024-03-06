using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Jump_TouchInput: MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cylindar;// Assign your cylinder object in the inspector
    [SerializeField] private float rotationSpeed = 0.5f; // Adjust rotation sensitivity

    private Vector2 previousTouchPosition; // To store the last touch position

    public void OnPointerDown(PointerEventData eventData)
    {
        previousTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDelta = eventData.position - previousTouchPosition;

        float rotationAmount = - touchDelta.x * rotationSpeed * Time.deltaTime;
        cylindar.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
        previousTouchPosition = eventData.position;
        
        if(touchDelta.x > 2f) player.transform.localScale = new Vector3(1, 1, 1);
         else if (touchDelta.x < -2f) player.transform.localScale = new Vector3(-1, 1, 1);;
    }

    public void ResetRotation()
    {
        cylindar.transform.localEulerAngles = Vector3.zero;
    }
}

// : MonoBehaviour, IPointerMoveHandler//, IPointerExitHandler
// {
//     [SerializeField] GameObject player;
//     [SerializeField] GameObject cylindar;
//
//     [SerializeField] public Quaternion targetRotation;
//     [SerializeField] float updateSpeed;
//     [SerializeField] float scrollSpeed;
//
//     void Update()
//     {
//         Quaternion currentRotation = cylindar.transform.rotation;
//         cylindar.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, updateSpeed * Time.deltaTime);
//     }
//
//     public void OnPointerMove(PointerEventData eventData) 
// 	{
//         float diff = (eventData.delta.x);
//         Vector3 originalAngle = cylindar.transform.localEulerAngles;
//         originalAngle.y -= diff * scrollSpeed;
//         targetRotation = originalAngle;
//
//         if(diff > 2f) player.transform.localScale = new Vector3(1, 1, 1);
//         else if (diff < -2f) player.transform.localScale = new Vector3(-1, 1, 1);;
//         print(diff);
// 	}
//     
//     // public void OnPointerExit(PointerEventData eventData) {
//     //     targetRotation = cylindar.transform.localEulerAngles;
//     // }
// }
