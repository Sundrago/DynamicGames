using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Land_PointerEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Land_GameManager rocket;

    public string idx;

    public void OnPointerUp(PointerEventData eventData)
    {
        rocket.ChangeBtnState(idx, false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rocket.ChangeBtnState(idx, true);
    }

}
