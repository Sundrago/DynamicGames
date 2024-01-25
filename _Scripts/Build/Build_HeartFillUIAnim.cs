using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Build_HeartFillUIAnim : MonoBehaviour
{
    [SerializeField] public GameObject fill;

    public bool isFilled = true;

    public void SetFill(bool flag) {
        if(isFilled == flag) return;

        if(flag) {
            fill.transform.localScale = Vector3.zero;
            fill.transform.DOScale(new Vector3(1f,1f,1f), 0.5f)
                .SetEase(Ease.InOutElastic);
        } else {
            gameObject.transform.DOPunchScale(new Vector3(1f,1f,1f), 0.2f,10,0.5f);
            fill.transform.localScale = new Vector3(1f,1f,1f);
            fill.transform.DOScale(new Vector3(0f,0f,0f), 0.3f)
                .SetEase(Ease.InOutElastic);
        }
        isFilled = flag;
    }
}
