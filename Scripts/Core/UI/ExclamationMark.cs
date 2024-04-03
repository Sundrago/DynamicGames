using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExclamationMark : MonoBehaviour
{
    [SerializeField] private Camera camera;
    
    private const float OffsetX = 0;
    private const float OffsetY = 140;
    private bool initialized = false;
    private Action callback = null;
    private Transform target;

    [Button]
    public void Init(Transform target, Action callback=null)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector2 defaultSizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
        rect.sizeDelta = Vector2.zero;

        rect.DOSizeDelta(defaultSizeDelta, 1f);
        
        initialized = true;
        this.target = target;
        this.callback = callback;
        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!initialized)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.transform.position = target.position;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + OffsetX,
            gameObject.transform.localPosition.y + OffsetY, gameObject.transform.localPosition.z);
    }
    
    public void Clicked()
    {
        callback.Invoke();
        Destroy(gameObject);
        // gameObject.SetActive(false);
    }
}
