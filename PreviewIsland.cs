using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PreviewIsland : MonoBehaviour
{
    [SerializeField] private RectTransform rect, notch;
    [SerializeField] private Transform notchPos;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private List<GameObject> boards;
    
    public static PreviewIsland Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    [Button]
    public void Open(int idx)
    {
        if (!gameObject.activeSelf)
        {
            DOTween.Kill(rect);
            rect.DOSizeDelta(new Vector2(700, rect.sizeDelta.y), 0.45f).SetEase(Ease.InCubic);
        }
        rect.DOSizeDelta(new Vector2(700, 700), 0.5f).SetEase(Ease.OutBack).SetDelay(0.35f);
        
        for (int i = 0; i < boards.Count; i++)
        {
            boards[i].gameObject.SetActive(i == idx);
        }
        
        gameObject.SetActive(true);
        rect.sizeDelta = notch.sizeDelta;
        gameObject.transform.position = notchPos.position;

        switch (idx)
        {
            case 0:
                typewriter.ShowText(MyUtility.Localize.GetLocalizedString("[previewIsland_0] Fluffy를 드래그해서 \n프렌즈 블록 위에 놓아보세요."));
                break;
            case 1:
                typewriter.ShowText(MyUtility.Localize.GetLocalizedString("[previewIsland_1] Fluffy를 게임 블록 위에 놓아보세요."));
                break;
            case 2:
                typewriter.ShowText(MyUtility.Localize.GetLocalizedString("[previewIsland_2] 게임을 터치해서 \n플레이하세요!"));
                break;
        }
        typewriter.StartShowingText(true);
    }

    [Button]
    public void Close()
    {
        DOTween.Kill(rect);
        rect.DOSizeDelta(new Vector2(750, notch.sizeDelta.y), 0.4f).SetEase(Ease.InOutCubic);
        rect.DOSizeDelta(notch.sizeDelta, 0.5f).SetEase(Ease.InOutCubic).SetDelay(0.3f)
            .OnComplete(()=>
            {
                gameObject.SetActive(false);
            });
    }
}
