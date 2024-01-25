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
    
    private Transform targetTransform;
    public float offsetY;
    private float endTime;
    private DialogueStatus status = DialogueStatus.hidden;

    private enum DialogueStatus
    {
        appear, disappear, hidden
    }
    
    [Button]
    public void Init(string input, Transform _targetTransform, bool forceShow = false, float _offsetY = 0.3f, float duration = 5)
    {
        if(status != DialogueStatus.hidden && !forceShow) return;

        DOTween.Kill(main_ui);
        DOTween.Kill(bgImage);
        
        outline_ui.color = Color.clear;
        outline_ui.text = Regex.Replace(input, "<.*?>", string.Empty);
        
        main_ui.text = input;
        targetTransform = _targetTransform;
        offsetY = _offsetY;
        endTime = Time.time + duration;

        bgImage.color = new Color(0, 0, 0, 0);
        bgImage.DOFade(0.8f, 0.5f);
        main_ui.color = new Color(1, 1, 1, 0);
        main_ui.DOFade(0.8f, 0.2f);
        
        gameObject.SetActive(true);
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
                status = DialogueStatus.disappear;
                typewriter.StartDisappearingText();
                bgImage.DOFade(0, 2f).OnComplete(
                    ()=>
                    {
                        status = DialogueStatus.hidden;
                        gameObject.SetActive(false);
                    });
                main_ui.DOFade(0, 2.5f);
            }
        }
    }
}
