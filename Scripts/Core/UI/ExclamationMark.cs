using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExclamationMark : MonoBehaviour
{
    [SerializeField] private float offsetX, offsetY;
    [SerializeField] private Camera camera;
    
    private Transform target;
    private bool initialized = false;
    
    public delegate void Callback();
    private Callback callback = null;

    [Button]
    public void Init(Transform _target, Callback _callback=null)
    {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        Vector2 defaultSizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
        rect.sizeDelta = Vector2.zero;

        rect.DOSizeDelta(defaultSizeDelta, 1f);
        
        initialized = true;
        target = _target;
        callback = _callback;
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
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + offsetX,
            gameObject.transform.localPosition.y + offsetY, gameObject.transform.localPosition.z);
    }
    
    public void Clicked()
    {
        callback.Invoke();
        Destroy(gameObject);
        // gameObject.SetActive(false);
    }
}
