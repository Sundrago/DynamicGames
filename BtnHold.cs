using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool mouseDown = false;
    public float startTime;

    private void Update()
    {
        if (!mouseDown) return;
        if (gameObject.GetComponent<Button>().interactable == false) return;

        if (startTime + 0.5f > Time.time) return;
        else if(startTime + 2f > Time.time)
        {
            if (Time.frameCount % 8 == 0) gameObject.GetComponent<Button>().onClick.Invoke();
        } else if((startTime + 4f > Time.time)) {
            if (Time.frameCount % 4 == 0) gameObject.GetComponent<Button>().onClick.Invoke();
        } else if (Time.frameCount % 2 == 0) gameObject.GetComponent<Button>().onClick.Invoke();


    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        startTime = Time.time;
        mouseDown = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        mouseDown = false;
    }
}