using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class IslandSizeCtrl : MonoBehaviour
{
    [SerializeField] RectTransform samllsized;
    private RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
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
    }
}
