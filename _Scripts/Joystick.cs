using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] GameObject joystickUI;
    [SerializeField] Sprite[] joystickImg = new Sprite[4];
    [SerializeField] RocketPhysics rocket;

    private int currentImg = 0;
    private Vector2 initialPoint;

    private void Start()
    {
        SetState(0);
        joystickUI.SetActive(false);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        SetState(0);
        initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        joystickUI.transform.position = new Vector2(initialPoint.x, initialPoint.y + joystickUI.GetComponent<RectTransform>().transform.localScale.y / 2f);
        joystickUI.SetActive(true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePos.y += joystickUI.GetComponent<RectTransform>().transform.localScale.y / 2f;
        //joystickUI.transform.position = mousePos;
        float degree = Mathf.Atan2(initialPoint.y - mousePos.y, initialPoint.x - mousePos.x) * Mathf.Rad2Deg;
        float dist = Vector2.Distance(initialPoint, mousePos);
        if (dist < joystickUI.GetComponent<RectTransform>().transform.localScale.y / 2f)
        {
            SetState(0);
        }
        else if (degree > -60f & degree < 0) SetState(1);
        else if (degree > -120f & degree < -60f) SetState(2);
        else if (degree > -180f & degree < -120f) SetState(3);
        else SetState(0);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        SetState(0);
        joystickUI.SetActive(false);
    }

    void SetState(int idx)
    {
        if(currentImg != idx)
        {
            currentImg = idx;
            joystickUI.GetComponent<Image>().sprite = joystickImg[idx];
        }
        switch(idx)
        {
            case 0: //idle
                rocket.btn_up = false;
                rocket.btn_left = false;
                rocket.btn_right = false;
                break;
            case 1:
                rocket.btn_up = false;
                rocket.btn_left = true;
                rocket.btn_right = false;
                break;
            case 2:
                rocket.btn_up = true;
                rocket.btn_left = false;
                rocket.btn_right = false;
                break;
            case 3:
                rocket.btn_up = false;
                rocket.btn_left = false;
                rocket.btn_right = true;
                break;
        }
    }
}