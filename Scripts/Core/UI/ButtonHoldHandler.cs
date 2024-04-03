using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonHoldHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isMouseDown;
    private float startTime;
    private Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (!isMouseDown || !button.interactable) return;
        if (startTime + 0.5f > Time.time) return;
        
        else if(startTime + 2f > Time.time)
        {
            if (Time.frameCount % 8 == 0) 
                button.onClick.Invoke();
        } else if((startTime + 4f > Time.time)) {
            if (Time.frameCount % 4 == 0) 
                button.onClick.Invoke();
        } else if (Time.frameCount % 2 == 0) 
            button.onClick.Invoke();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        startTime = Time.time;
        isMouseDown = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        isMouseDown = false;
    }
}