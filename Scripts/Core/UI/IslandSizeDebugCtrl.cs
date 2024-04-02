using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IslandSizeDebugCtrl : MonoBehaviour
{
    [SerializeField] private IslandSizeController islandsize;
    [SerializeField] private TextMeshProUGUI debugTextUI;

    private void OnEnable()
    {
        UpdateString();
        islandsize.GetComponent<Image>().color = Color.red;
    }

    private void UpdateString()
    {
        string output = "";
        output += "width : " + islandsize.smallsized.sizeDelta.x + "\n";
        output += "height : " + islandsize.smallsized.sizeDelta.y + "\n";
        output += "x : " + islandsize.smallsized.anchoredPosition.x + "\n";
        output += "y : " + islandsize.smallsized.anchoredPosition.y;
        debugTextUI.text = output;
        islandsize.CloseIsland();
    }

    public void AdjustWidth(float amount)
    {
        islandsize.smallsized.sizeDelta =
            new Vector2(islandsize.smallsized.sizeDelta.x + amount, islandsize.smallsized.sizeDelta.y);
        UpdateString();
    }
    
    public void AdjustHeight(float amount)
    {
        islandsize.smallsized.sizeDelta =
            new Vector2(islandsize.smallsized.sizeDelta.x, islandsize.smallsized.sizeDelta.y + amount);
        UpdateString();
    }
    
    public void AdjjustPosX(float amount)
    {
        islandsize.smallsized.anchoredPosition =
            new Vector2(islandsize.smallsized.anchoredPosition.x + amount, islandsize.smallsized.anchoredPosition.y);
        UpdateString();
    }
    
    public void AdjjustPosY(float amount)
    {
        islandsize.smallsized.anchoredPosition =
            new Vector2(islandsize.smallsized.anchoredPosition.x, islandsize.smallsized.anchoredPosition.y + amount);
        UpdateString();
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
