using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shoot_BG : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField] float offsetAmount;
    private Vector3 originalPos;

    private void Start()
    {
        originalPos = gameObject.transform.position;
    }

    private void Update()
    {
        gameObject.transform.position = player.transform.position * offsetAmount;
    }
}
