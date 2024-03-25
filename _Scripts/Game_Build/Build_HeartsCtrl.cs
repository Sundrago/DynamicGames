using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Represents the controller for managing the UI and logic of the hearts in the game.
/// </summary>
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

    /// <summary>
    /// Sets the number of filled hearts in the UI representation.
    /// </summary>
    /// <param name="idx">The number of filled hearts.</param>
    public void SetHearts(int idx)
    {
        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].IsFilled = (i < idx);
        }
    }

    /// <summary>
    /// Shows or hides the UI representation of the hearts.
    /// </summary>
    /// <param name="show">If set to true, the hearts will be shown. If set to false, the hearts will be hidden.</param>
    public void Show(bool show)
    {
        foreach (Build_HeartFillUIAnim heart in hearts) 
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
