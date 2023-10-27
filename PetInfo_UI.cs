using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using TMPro;

public class PetInfo_UI : MonoBehaviour
{
    [SerializeField]
    private Image previewImage;

    [SerializeField]
    private TextMeshProUGUI name_ui, level_ui, age_ui, skills_ui;
    [SerializeField]
    private TextMeshProUGUI exp_ui;
    [SerializeField]
    private RectMask2D expSlider_ui;

    [SerializeField]
    private Transform panel;
    
    [SerializeField]
    private float scaleOffset = 1;
    [SerializeField]
    private float yPosOffsetScale = 1;
    [SerializeField]
    private float yPosOffset = 0;
    
    private SpriteRenderer spriteRenderer;
    private PetType type;

    [SerializeField]
    private TextMeshProUGUI updateText;
    
    [SerializeField]
    private float sliderSizeDeltaX;
    // private void Start()
    // {
    //     sliderSizeDeltaX = expSlider_ui.gameObject.GetComponent<RectTransform>().sizeDelta.x;
    // }

    private void Update()
    {
        if (Time.frameCount % 5 == 0)
        {
            if (spriteRenderer == null || !spriteRenderer.gameObject.activeSelf)
                return;

            if(previewImage.sprite != spriteRenderer.sprite)
                previewImage.sprite = spriteRenderer.sprite;
            previewImage.gameObject.transform.localScale = spriteRenderer.gameObject.transform.localScale * scaleOffset;
        }
    }

    [Button]
    public void SetTarget(PetType _type)
    {
        type = _type;

        //preview Image
        spriteRenderer = PetManager.Instance.GetPetDataByType(_type).obj.GetComponent<Pet>().spriteRenderer;
        previewImage.sprite = spriteRenderer.sprite;
        previewImage.gameObject.transform.localScale = spriteRenderer.gameObject.transform.localScale * scaleOffset;
        previewImage.gameObject.transform.localPosition = new Vector3(0, spriteRenderer.gameObject.transform.localPosition.y * yPosOffsetScale + yPosOffset, 0);

        
        //NO pet
        if (PetManager.Instance.GetPetCount(_type) == 0)
        {
            previewImage.color = Color.black;
            name_ui.text = "????";
            level_ui.text = "????";
            age_ui.text = "????";
            skills_ui.text = "????";
            
            expSlider_ui.padding = new Vector4(0, 0, sliderSizeDeltaX, 0);
            exp_ui.text = "?";
            return;
        }
        previewImage.color = Color.white;
        
        
        int exp = PetManager.Instance.GetPetExp(_type);
        int level = PetManager.Instance.GetPetLevel(_type);
        
        name_ui.text = _type.ToString();
        level_ui.text = "Level : " + level;
        age_ui.text = "Age : " + PetManager.Instance.GetPetAge(_type);
        skills_ui.text = "Skills : not found yet";

        float expNormal = exp / (level * 5f);
        expSlider_ui.padding = new Vector4(0, 0, sliderSizeDeltaX - sliderSizeDeltaX * expNormal, 0);
        exp_ui.text = exp + "/" + (level * 5);
    }

    public void LevelUP()
    {
        if (PetManager.Instance.PetLevelUP(type))
        {
            SetTarget(type);
        }
        else
        {
            expSlider_ui.gameObject.transform.localPosition = new Vector3(0, expSlider_ui.gameObject.transform.localPosition.y, 0);
            DOTween.Kill(expSlider_ui.gameObject.transform);
            expSlider_ui.gameObject.transform.DOPunchPosition(new Vector3(5, 0, 0), 1f, 5);
        }
    }

    public void ShowPanel(PetType _type)
    {
        SetTarget(_type);
        if(gameObject.activeSelf) return;
        panel.localPosition = new Vector3(0, -1500, 0);
        panel.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutExpo);
        gameObject.SetActive(true);
        updateText.gameObject.SetActive(false);
    }

    public void HidePanel(bool longTransition = false)
    {
        if (DOTween.IsTweening(panel) || gameObject.activeSelf == false)
            return;
        
        panel.DOLocalMoveY(-1500, longTransition? 1f : 0.5f).SetEase(Ease.OutExpo)
            .OnComplete(()=>{gameObject.SetActive(false);});
    }

    public void Hidden()
    {
        panel.localPosition = new Vector3(0, -1500, 0);
        gameObject.SetActive(false);
    }

    public void FeedBtnClicked()
    {
        updateText.color = Color.white;
        updateText.gameObject.SetActive(true);
        updateText.DOFade(0, 2f)
            .From();
    }
}