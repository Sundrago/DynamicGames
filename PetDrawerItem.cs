using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class PetDrawerItem : MonoBehaviour
{
    [SerializeField] private Image image_ui, image_mask;
    [SerializeField] private TextMeshProUGUI name_ui, level_ui;
    [SerializeField] private RectMask2D rectMask2D;
    [SerializeField]
    private GameObject sliderobj;
    private PetType type;
    private string name;
    
    
    public void Init(PetType _type, Sprite _sprite, string _name, float size = 300f, float relativePosY = 0)
    {
        image_ui.sprite = _sprite;
        image_ui.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        image_ui.rectTransform.anchoredPosition = new Vector2(0, relativePosY);
        name = _name;
        type = _type;
        
        image_ui.color = Color.black;
        name_ui.text = "?";
        sliderobj.SetActive(false);
        level_ui.text = "";
        image_mask.color = Color.gray;
        image_mask.gameObject.SetActive(true);
        
        gameObject.SetActive(true);
    }

    public void UpdateItem(PetType _type)
    {
        type = _type;
        int petCount = PetManager.Instance.GetPetCountByType(type);
        print(type + " : " +  petCount);
        if (petCount == 0)
        {
            image_ui.color = Color.black;
            name_ui.text = "?";
            sliderobj.SetActive(false);
            level_ui.text = "";
            image_mask.color = Color.gray;
            image_mask.gameObject.SetActive(true);
        }
        else
        {
            image_ui.color = Color.white;
            name_ui.text = type.ToString();
            sliderobj.SetActive(true);
            SetSlider(petCount);
            image_mask.gameObject.SetActive(false);
            level_ui.gameObject.SetActive(true);
        }
    }
    
    public void UpdateItemWithAnim()
    {
        int petCount = PetManager.Instance.GetPetCountByType(type);
        image_ui.color = Color.white;
        name_ui.text = type.ToString();
        sliderobj.SetActive(true);
        image_mask.DOFade(0, 0.5f)
            .OnComplete(() => {
                image_mask.gameObject.SetActive(false);
            });

        level_ui.gameObject.SetActive(true);
        DOVirtual.Float(petCount - 1F, petCount, 0.5F, SetSlider).SetEase(Ease.OutExpo);
    }
    
    [Button]
    public void SetSlider(float amt)
    {
        float value = amt / 10f;
        rectMask2D.padding = new Vector4(0, 0, 190 - 190 * value, 0);
        level_ui.text = Mathf.Round(amt) + "/10";
    }
}
