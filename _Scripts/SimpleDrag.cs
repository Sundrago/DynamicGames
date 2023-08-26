using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SimpleDrag : MonoBehaviour
{
    Vector2 startPosition;
    bool isDrag = false;

    public void OnMouseDown()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        startPosition = eventDataCurrentPosition.position;

        isDrag = true;
    }

    void OnMouseDrag()
    {
        if (isDrag)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        isDrag = false;
    }
}
