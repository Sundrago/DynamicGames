using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoResizeCapsuleCollider : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D collider2D;
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private bool fixedWidth = false;
    
    void Update()
    {
        if(Time.frameCount%60 != 0) return;
        var rect = rectTransform.rect;
        if(collider2D!=null)
            collider2D.size = new Vector2 (fixedWidth? collider2D.size.x : rect.width, rect.height);
        if(boxCollider2D!=null)
            boxCollider2D.size = new Vector2 (fixedWidth? boxCollider2D.size.x : rect.width, rect.height);
    }
}
