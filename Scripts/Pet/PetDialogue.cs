using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Febucci.UI;
using Sirenix.OdinInspector;
using System.Text.RegularExpressions;

public class PetDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI outline_ui, main_ui;
    [SerializeField] private Image bgImage;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Transform dialogueLeft, dialogueRight, canvasLeft, canvasRight, tails;
    [SerializeField] private float diff;
    [SerializeField] private Image topTail, btmTail;
    
    private Transform targetTransform;
    public float offsetY;
    private float endTime;
    private DialogueStatus status = DialogueStatus.hidden;

    private enum DialogueStatus
    {
        appear, disappear, hidden
    }
    
    [Button]
    public void Init(string input, Transform _targetTransform, bool forceShow = false, float _offsetY = 0.4f, float duration = 5)
    {
        if(status != DialogueStatus.hidden && !forceShow) return;
        if(!_targetTransform.gameObject.activeSelf) return;

        DOTween.Kill(main_ui);
        DOTween.Kill(bgImage);

        if (_offsetY > 0)
        {
            topTail.gameObject.SetActive(false);
            btmTail.gameObject.SetActive(true);
            DOTween.Kill(btmTail);
            btmTail.DOFade(0.8f, 0.3f);
        }
        else
        {
            topTail.gameObject.SetActive(true);
            btmTail.gameObject.SetActive(false);
            DOTween.Kill(topTail);
            topTail.DOFade(0.8f, 0.3f);
        }
        
        outline_ui.color = Color.clear;
        outline_ui.text = Regex.Replace(input, "<.*?>", string.Empty);
        
        targetTransform = _targetTransform;
        offsetY = _offsetY;
        endTime = Time.time + duration;

        bgImage.color = new Color(0, 0, 0, 0);
        bgImage.DOFade(0.8f, 0.5f);
        main_ui.color = new Color(1, 1, 1, 0);
        main_ui.DOFade(0.8f, 0.3f);
        
        gameObject.SetActive(true);
        typewriter.ShowText(input);
        typewriter.StartShowingText(true);
        status = DialogueStatus.appear;

        rect.SetAsLastSibling();
    }

    private void Update()
    {
        if (status == DialogueStatus.hidden)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.transform.position = targetTransform.position;
        gameObject.transform.Translate(new Vector3(0,offsetY, 0));

        if (status == DialogueStatus.appear)
        {
            if (Time.time > endTime)
            {
                Hide();
            }
        }
        
        //RePos
        if (dialogueLeft.transform.position.x < canvasLeft.transform.position.x + diff)
        {
            gameObject.transform.Translate(canvasLeft.transform.position.x + diff - dialogueLeft.transform.position.x, 0, 0);
        } else if (dialogueRight.transform.position.x > canvasRight.transform.position.x - diff)
        {
            gameObject.transform.Translate(-(dialogueRight.transform.position.x - (canvasRight.transform.position.x - diff)), 0, 0);
        }

        tails.transform.position = new Vector3(targetTransform.position.x, tails.transform.position.y, 0);
    }

    public void Hide()
    {
        if(status!=DialogueStatus.appear) return;
        
        DOTween.Kill(main_ui);
        DOTween.Kill(bgImage);
        DOTween.Kill(topTail);
        DOTween.Kill(btmTail);
        
        status = DialogueStatus.disappear;
        typewriter.StartDisappearingText();
        bgImage.DOFade(0, 2f).OnComplete(
            ()=>
            {
                status = DialogueStatus.hidden;
                gameObject.SetActive(false);
            });
        main_ui.DOFade(0, 2.5f);
        topTail.DOFade(0, 2.5f);
        btmTail.DOFade(0, 2.5f);
    }
}
