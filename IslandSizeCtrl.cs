using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class IslandSizeCtrl : MonoBehaviour
{
    [SerializeField] RectTransform samllsized;
    [SerializeField] Image[] faceImgs;

    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        foreach (Image img in faceImgs)
        {
            img.DOFade(0f, 0f);
        }

        rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, samllsized.sizeDelta.y), 0f)
            .OnUpdate(() => {
                Vector2 pos = rect.anchoredPosition;
                pos.y = rect.sizeDelta.y * -1f / 2f - 33.7f;
                rect.anchoredPosition = pos;
            });
    }

    [Button]
    public void OpenIsland()
    {
        rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, rect.sizeDelta.x), 1.5f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(()=> {
                Vector2 pos = rect.anchoredPosition;
                pos.y = rect.sizeDelta.y * -1f / 2f - 33.7f;
                rect.anchoredPosition = pos;
            });

        foreach(Image img in faceImgs)
        {
            img.DOFade(1f, 1f).SetEase(Ease.OutExpo);
        }
    }

    [Button]
    public void CloseIsland()
    {
        rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, samllsized.sizeDelta.y), 1.5f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(() => {
                Vector2 pos = rect.anchoredPosition;
                pos.y = rect.sizeDelta.y * -1f / 2f - 33.7f;
                rect.anchoredPosition = pos;
            });

        foreach (Image img in faceImgs)
        {
            img.DOFade(0f, 1f).SetEase(Ease.OutExpo);
        }
    }
}
