using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FruitController : MonoBehaviour
{
    [SerializeField] public Image fruitImage;
    [SerializeField] private Button fruitBtn;
    [SerializeField] private TextMeshProUGUI fruitCount_UI;

    public void Init(int amount)
    {
        fruitCount_UI.text = amount.ToString();
        fruitBtn.interactable = (amount != 0);
    }
}
