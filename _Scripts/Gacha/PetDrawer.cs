using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PetDrawer : SerializedMonoBehaviour
{
    [SerializeField] private PetManager petManager;
    [SerializeField] private PetDrawerItem petDrawerItem_prefab;
    public Dictionary<PetType, PetDrawerItem> drawerItems = new Dictionary<PetType, PetDrawerItem>();
    [SerializeField] private Transform draweritemHolder;
    [SerializeField] private int height, width = 300;
    [SerializeField] private float sizeFactor, posFactor;
    [SerializeField] private float startHeight = 300;
    [SerializeField] private RectTransform contents;
    [SerializeField] private Image bgBtn;
    [SerializeField] private float sliderOffset;
    [SerializeField] private float heightOffset;
    [FormerlySerializedAs("petInfo")]
    [SerializeField] private PetInfo_UI petInfoUI;
    
    private float panelHeight;

    private void Start()
    {
        panelHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
        UpdateItems();
        gameObject.SetActive(false);
        petInfoUI.gameObject.SetActive(false);
    }

    private void UpdateItems()
    {
        foreach (KeyValuePair<PetType, PetDrawerItem> item in drawerItems)
        {
            item.Value.UpdateItem(item.Key);
        }
    }
    
    public Transform GetItemTransformByType(PetType _type)
    {
        if (!drawerItems.ContainsKey(_type))
        {
            print("GetItemTransformByType : " + _type + " Not Found");
            return null;
        }
        return drawerItems[_type].transform;
    }

    public void PetItenClicked(PetType _petType)
    {
        petInfoUI.ShowPanel(_petType);
    }
    
#if UNITY_EDITOR
    [Button]
    
    private void SetDrawer()
    {
        foreach (KeyValuePair<PetType, PetDrawerItem> item in drawerItems)
        {
            if (item.Value != null) 
                item.Value.gameObject.SetActive(false);
            Destroy(item.Value.gameObject);
        }
        drawerItems = new Dictionary<PetType, PetDrawerItem>();
        
        //create
        for (int i = 0; i<petManager.petdatas.Count; i++)
        {
            Petdata data = petManager.petdatas[i];
            PetDrawerItem item = Instantiate(petDrawerItem_prefab, draweritemHolder);

            int x = i % 4;
            int y = (i - x) / 4;
            
            float relativeSize = data.obj.GetComponent<Pet>().spriteRenderer.gameObject.transform.localScale.x * sizeFactor * 300f;
            float relativePosY = data.obj.GetComponent<Pet>().spriteRenderer.gameObject.transform.localPosition.y  * posFactor;
            
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(width * -1.5f + width * x , - height * y + height / 2f + startHeight);
            
            item.Init(data.type,data.image,data.type.ToString(), Mathf.Abs(relativeSize), relativePosY);
            drawerItems.Add(data.type, item);
        }

        float contentsHeight = ((petManager.petdatas.Count - petManager.petdatas.Count % 4) / 4 + 1) * height + heightOffset;
        if (petManager.petdatas.Count % 4 == 0) contentsHeight -= height;
        contents.sizeDelta = new Vector2(contents.sizeDelta.x, contentsHeight);
    }
#endif

    [Button]
    public void SlideToItemByIdx(PetType _petType)
    {
        PetDrawerItem item = drawerItems[_petType];
        float contentsHeight = (item.GetComponent<RectTransform>().anchoredPosition.y) * -1 + sliderOffset - panelHeight / 2f;
        // contents.anchoredPosition = new Vector2(0, contentsHeight);
        
        contents.DOAnchorPosY(contentsHeight, 0.1f)
            .SetEase(Ease.OutExpo);
    }

    [Button]
    public void ShowPanel(bool updateItem = true, bool small = false)
    {
        if(gameObject.activeSelf) return;
        if(DOTween.IsTweening(bgBtn)) return;

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.UI_OPEN);
        if (updateItem) UpdateItems();
        if (!small) contents.anchoredPosition = Vector2.zero;
        
        gameObject.SetActive(true);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
        bgBtn.color = Color.clear;

        rect.DOAnchorPosY(small ? panelHeight/2f : panelHeight, 0.5f)
            .SetEase(Ease.OutExpo);
        rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, small ? panelHeight : panelHeight * 2f), 0.5f)
            .SetEase(Ease.OutExpo);
        bgBtn.DOFade(0.3f, 0.55f);
        
        petInfoUI.Hidden();
    }

    [Button]
    public void HidePanel()
    {
        if(DOTween.IsTweening(bgBtn)) return;
        
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.UI_CLOSE);
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        
        rect.DOAnchorPosY(-rect.sizeDelta.y/2f, 0.3f)
            .SetEase(Ease.InExpo);
        rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, 0), 0.3f)
            .SetEase(Ease.InExpo);
        bgBtn.DOFade(0, 0.4f)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
        petInfoUI.HidePanel();
    }
}
