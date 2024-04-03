using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;


public class IslandSizeDebugController : MonoBehaviour
{
    [SerializeField] private IslandSizeController islandSizeController;
    [SerializeField] private TextMeshProUGUI debugTextUI;

    private void OnEnable()
    {
        UpdateString();
        islandSizeController.GetComponent<Image>().color = Color.red;
    }

    private void UpdateString()
    {
        string output = "";
        output += "width : " + islandSizeController.smallsized.sizeDelta.x + "\n";
        output += "height : " + islandSizeController.smallsized.sizeDelta.y + "\n";
        output += "x : " + islandSizeController.smallsized.anchoredPosition.x + "\n";
        output += "y : " + islandSizeController.smallsized.anchoredPosition.y;
        debugTextUI.text = output;
        islandSizeController.CloseIsland();
    }

    public void AdjustWidth(float amount)
    {
        islandSizeController.smallsized.sizeDelta =
            new Vector2(islandSizeController.smallsized.sizeDelta.x + amount, islandSizeController.smallsized.sizeDelta.y);
        UpdateString();
    }
    
    public void AdjustHeight(float amount)
    {
        islandSizeController.smallsized.sizeDelta =
            new Vector2(islandSizeController.smallsized.sizeDelta.x, islandSizeController.smallsized.sizeDelta.y + amount);
        UpdateString();
    }
    
    public void AdjjustPosX(float amount)
    {
        islandSizeController.smallsized.anchoredPosition =
            new Vector2(islandSizeController.smallsized.anchoredPosition.x + amount, islandSizeController.smallsized.anchoredPosition.y);
        UpdateString();
    }
    
    public void AdjjustPosY(float amount)
    {
        islandSizeController.smallsized.anchoredPosition =
            new Vector2(islandSizeController.smallsized.anchoredPosition.x, islandSizeController.smallsized.anchoredPosition.y + amount);
        UpdateString();
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
