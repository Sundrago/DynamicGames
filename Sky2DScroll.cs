using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class Sky2DScroll : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    private GameObject child;
    private RectTransform rect;

    public void Init(Sprite _sprite, float _scrollSpeed, float _transitionTime)
    {
        scrollSpeed = _scrollSpeed;
        
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.height * 1.78f, Screen.height);
        GetComponent<Image>().sprite = _sprite;

        //create child
        child = Instantiate(gameObject, gameObject.transform);
        Destroy(child.GetComponent<Sky2DScroll>());
        child.transform.position = gameObject.transform.position;

        //set child pos
        Vector2 childPos = child.GetComponent<RectTransform>().anchoredPosition;
        childPos.x -= rect.sizeDelta.x;
        child.GetComponent<RectTransform>().anchoredPosition = childPos;
        
        //Set transition fx
        GetComponent<Image>().color = new Color(1, 1, 1, 0);
        child.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        GetComponent<Image>().DOFade(1, _transitionTime);
        child.GetComponent<Image>().DOFade(1, _transitionTime);
    }
    
    void Update()
    {
        rect.anchoredPosition = new Vector2(
            rect.anchoredPosition.x + scrollSpeed * Time.deltaTime,
            rect.anchoredPosition.y
        );

        if (rect.anchoredPosition.x > rect.sizeDelta.x)
        {
            rect.anchoredPosition = new Vector2(
                rect.anchoredPosition.x - rect.sizeDelta.x,
                rect.anchoredPosition.y
            );
        }
    }
}