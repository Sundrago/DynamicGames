using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Build_HeartFillUIAnim : MonoBehaviour
{
    [SerializeField] public GameObject fill;
    [SerializeField] public Image heartFrame, heartFill;
    private bool isFilled = true;
    
    public bool IsFilled
    {
        get { return isFilled; }
        set
        {
            if (isFilled == value) 
                return;

            isFilled = value;
            SetFill(isFilled);
        }
    }
    
    private void SetFill(bool shouldFill) {
        if(shouldFill) {
            ApplyFillAnimation();
        } else {
            ApplyEmptyAnimation();
        }
    }

    private void ApplyFillAnimation() {
        fill.transform.localScale = Vector3.zero;
        fill.transform.DOScale(new Vector3(1f,1f,1f), 0.5f)
            .SetEase(Ease.InOutElastic);
    }

    private void ApplyEmptyAnimation() {
        gameObject.transform.DOPunchScale(new Vector3(1f,1f,1f), 0.2f,10,0.5f);
        fill.transform.localScale = new Vector3(1f,1f,1f);
        fill.transform.DOScale(new Vector3(0f,0f,0f), 0.3f)
            .SetEase(Ease.InOutElastic);
    }

    private void SetImageTransition(float endValue, float duration)
    {
        DOTween.Kill(heartFrame);
        DOTween.Kill(heartFill);
        heartFrame.DOFade(endValue, duration);
        heartFill.DOFade(endValue, duration);
    }
}
