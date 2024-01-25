using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shoot_face_move : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField]
    private RectTransform face;
    [SerializeField] private float offsetAmt;
    private Vector3 originalPos;

    private void Start()
    {
        face = GetComponent<RectTransform>();
        originalPos = face.anchoredPosition;
    }
    private void LateUpdate()
    {
        Vector2 vecNormal = (Vector2)gameObject.transform.position - (Vector2)player.position;
        face.anchoredPosition = originalPos + (Vector3)vecNormal.normalized * offsetAmt;
    }
}
