using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.Pet
{
    public class PetInventoryItem : MonoBehaviour
    {
        [FormerlySerializedAs("petInventory")] [Header("Managers and Controllers")] [SerializeField]
        private PetInventoryUIManager petInventoryUIManager;

        [Header("UI Components")] [SerializeField]
        private Image uiImage;

        [SerializeField] private Image maskImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private RectMask2D rectMask2D;
        [SerializeField] private GameObject sliderObject;

        private string name;
        private int petLevel;
        private PetType type;

        public void InitializePetInventoryItem(PetType _type, Sprite _sprite, string _name, float size = 300f,
            float relativePosY = 0)
        {
            uiImage.sprite = _sprite;
            uiImage.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
            uiImage.rectTransform.anchoredPosition = new Vector2(0, relativePosY);
            name = _name;
            type = _type;

            uiImage.color = Color.black;
            nameText.text = "?";
            sliderObject.SetActive(false);
            levelText.text = "";
            maskImage.color = Color.gray;
            maskImage.gameObject.SetActive(true);

            gameObject.SetActive(true);
        }

        public void UpdatePetInventoryItem(PetType type)
        {
            this.type = type;
            var petCount = PetManager.Instance.GetPetCount(type);

            if (petCount == 0)
                UpdateUnavailablePetItem();
            else
                UpdateAvailablePetItem();
        }

        private void UpdateUnavailablePetItem()
        {
            uiImage.color = Color.black;
            nameText.text = "?";
            sliderObject.SetActive(false);
            levelText.text = "";
            maskImage.color = Color.gray;
            maskImage.gameObject.SetActive(true);
        }

        private void UpdateAvailablePetItem()
        {
            petLevel = PetManager.Instance.GetPetLevel(type);
            uiImage.color = Color.white;
            nameText.text = type.ToString();
            sliderObject.SetActive(true);
            UpdateSliderValue(PetManager.Instance.GetPetExp(type));
            maskImage.gameObject.SetActive(false);
            levelText.gameObject.SetActive(true);
        }

        public void UpdateItemWithAnimation()
        {
            var petCount = PetManager.Instance.GetPetCount(type);
            petLevel = PetManager.Instance.GetPetLevel(type);
            uiImage.color = Color.white;
            nameText.text = type.ToString();
            sliderObject.SetActive(true);
            maskImage.DOFade(0, 0.5f)
                .OnComplete(() => { maskImage.gameObject.SetActive(false); });

            levelText.gameObject.SetActive(true);
            DOVirtual.Float(PetManager.Instance.GetPetExp(type) - 1F, PetManager.Instance.GetPetExp(type), 0.5F,
                UpdateSliderValue).SetEase(Ease.OutExpo);
        }

        public void OnInventoryItemClicked()
        {
            petInventoryUIManager.PetItemClicked(type);
        }

        public void UpdateSliderValue(float amt)
        {
            var value = amt / (petLevel * 5f);
            rectMask2D.padding = new Vector4(0, 0, 190 - 190 * value, 0);
            levelText.text = Mathf.Round(amt) + "/" + petLevel * 5;
        }
    }
}