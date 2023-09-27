using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_footstebobj : MonoBehaviour
{
    public float height;
    public Vector2 pos;

    void Start()
    {
        pos = gameObject.GetComponent<RectTransform>().position;
        height = gameObject.GetComponent<RectTransform>().sizeDelta.y;
    }
}
