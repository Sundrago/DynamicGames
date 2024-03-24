using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Build_HeartsCtrl : MonoBehaviour
{
    [SerializeField] Build_HeartFillUIAnim[] hearts = new Build_HeartFillUIAnim[5];
    void Start()
    {
        foreach(Build_HeartFillUIAnim heart in hearts) 
        {
            SetAlphaAnim(heart, false);
        }
        gameObject.SetActive(true);
    }

    public void SetHearts(int idx) {
        for(int i = 0; i<hearts.Length; i++) {
            hearts[i].IsFilled = (i < idx);
        }
    }

    public void Show(bool show) 
    {
        foreach(Build_HeartFillUIAnim heart in hearts) 
        {
            SetAlphaAnim(heart, show);
        }
        gameObject.SetActive(true);
    }

    private void SetAlphaAnim(Build_HeartFillUIAnim heart, bool show) 
    {
        float endValue = show ? 1f : 0f;
        float duration = show ? 0.15f : 1.5f;
        
        DOTween.Kill(heart.heartFill);
        heart.heartFill.DOFade(endValue, duration);
        DOTween.Kill(heart.heartFrame);
        heart.heartFrame.DOFade(endValue, duration);
    }

}
