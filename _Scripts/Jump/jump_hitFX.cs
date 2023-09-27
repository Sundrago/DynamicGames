using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class jump_hitFX : MonoBehaviour
{
    void Start()
    {
        gameObject.transform.DOScale(new Vector3(10f,0.01f,10f), 1.5f)
            .SetEase(Ease.OutQuint);

        Material mat = gameObject.GetComponent<MeshRenderer>().material;
        Color color = mat.color;
        color.a = 0.7f;

        mat.DOColor(color, 0.5f);

        color.a = 0.01f;
        mat.DOColor(color, 1f)
            .SetDelay(0.5f)
            .SetEase(Ease.OutQuad)
        .OnComplete(()=>{Destroy(gameObject);});
    }
}
