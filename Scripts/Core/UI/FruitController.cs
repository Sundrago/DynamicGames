using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class FruitController : MonoBehaviour
    {
        [SerializeField] public Image fruitImage;
        [SerializeField] private Button fruitBtn;
        [SerializeField] private TextMeshProUGUI fruitCount_UI;

        public void Init(int amount)
        {
            fruitCount_UI.text = amount.ToString();
            fruitBtn.interactable = amount != 0;
        }
    }
}