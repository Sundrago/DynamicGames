using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeartsCtrl : MonoBehaviour
{
    [SerializeField] GameObject[] hearts = new GameObject[5];

    public void SetHearts(int idx) {
        for(int i = 0; i<hearts.Length; i++) {
            if(i < idx) hearts[i].GetComponent<HeartFillCtrl>().SetFill(true);
            else hearts[i].GetComponent<HeartFillCtrl>().SetFill(false);
        }
    }

    public void Show(bool show) {
        foreach(GameObject obj in hearts) {
            SetAlphaAnim(obj, show, 0.5f);
            SetAlphaAnim(obj.GetComponent<HeartFillCtrl>().fill, show, 1.5f);
        }
        gameObject.SetActive(true);
    }

    private void SetAlphaAnim(GameObject obj, bool show, float fadeOutTime) {
        DOTween.Kill(obj.GetComponent<Image>());
        obj.GetComponent<Image>().DOFade(show ? 1f : 0f, show ? 0.15f : 1.5f);
    }

    void Start()
    {
        foreach(GameObject obj in hearts) {
            Color color = obj.GetComponent<Image>().color;
            color.a =0f;
            obj.GetComponent<Image>().color = color;

            color = obj.GetComponent<HeartFillCtrl>().fill.GetComponent<Image>().color;
            color.a = 0f;
            obj.GetComponent<HeartFillCtrl>().fill.GetComponent<Image>().color = color;
        }
        gameObject.SetActive(true);
    }
}
