using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleDrag : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private float tolerance;
    
    Vector2 startMousePosition, startObjectPosition;
    bool isDrag = false;

    public void OnMouseDown()
    {
        startMousePosition = Input.mousePosition;
        startObjectPosition = rectTransform.anchoredPosition;
        
        isDrag = true;
    }

    private void LateUpdate()
    {
        if(!isDrag) return;

        if (Vector2.Distance(startMousePosition, Input.mousePosition) > tolerance)
        {
            isDrag = false;
            return;
        }
        
        Vector2 mouseOffset = startMousePosition - (Vector2)Input.mousePosition;
        rectTransform.anchoredPosition = startObjectPosition - mouseOffset;
    }

    private void OnMouseUp()
    {
        isDrag = false;
    }
}
