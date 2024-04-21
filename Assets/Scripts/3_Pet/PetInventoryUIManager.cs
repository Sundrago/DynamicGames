using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DynamicGames.System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Represents a pet inventory that manages the display and interaction with pets.
    /// </summary>
    public class PetInventoryUIManager : SerializedMonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private PetManager petManager;
        [SerializeField] private PetInventoryItem petInventoryItemPrefab;
        [SerializeField] private Transform drawerItemHolder;

        [Header("UI Components")] 
        [SerializeField] private RectTransform contents;
        [SerializeField] private Image bgBtn;
        [SerializeField] private PetInfoPanelManager petInfoPanelManager;
        
        [Header("Constants")] 
        private const int Height = 350;
        private const int Width = 300;
        private const float SizeFactor = 0.85f;
        private const float PosFactor = 800;
        private const float StartHeight = -310;
        private const float SliderOffset = 60;
        private const float HeightOffset = -40;
        
        public Dictionary<PetType, PetInventoryItem> drawerItems = new();
        private float panelHeight;

        private void Start()
        {
            panelHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            UpdateItems();
        }

        private void UpdateItems()
        {
            foreach (var item in drawerItems) item.Value.UpdatePetInventoryItem(item.Key);
            gameObject.SetActive(false);
            petInfoPanelManager.gameObject.SetActive(false);
        }

        public Transform GetItemTransformByType(PetType _type)
        {
            if (!drawerItems.ContainsKey(_type))
            {
                Debug.LogWarning("GetItemTransformByType : " + _type + " Not Found");
                return null;
            }

            return drawerItems[_type].transform;
        }

        public void PetItemClicked(PetType _petType)
        {
            petInfoPanelManager.ShowPanel(_petType);
        }

        [Button]
        public void SlideToItemByIdx(PetType _petType)
        {
            var item = drawerItems[_petType];
            var contentsHeight = item.GetComponent<RectTransform>().anchoredPosition.y * -1 + SliderOffset -
                                 panelHeight / 2f;

            contents.DOAnchorPosY(contentsHeight, 0.1f)
                .SetEase(Ease.OutExpo);
        }

        [Button]
        public void ShowPanel(bool updateItem = true, bool isSmallWindow = false)
        {
            if (gameObject.activeSelf) return;
            if (DOTween.IsTweening(bgBtn)) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Open);
            if (updateItem) UpdateItems();
            if (!isSmallWindow) contents.anchoredPosition = Vector2.zero;

            InitiateOpenAnimation(isSmallWindow);
            petInfoPanelManager.Hidden();
            gameObject.SetActive(true);
        }

        private void InitiateOpenAnimation(bool isSmallWindow)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);
            bgBtn.color = Color.clear;

            rect.DOAnchorPosY(isSmallWindow ? panelHeight / 2f : panelHeight, 0.5f)
                .SetEase(Ease.OutExpo);
            rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, isSmallWindow ? panelHeight : panelHeight * 2f), 0.5f)
                .SetEase(Ease.OutExpo);
            bgBtn.DOFade(0.3f, 0.55f);
        }

        [Button]
        public void HidePanel()
        {
            if (DOTween.IsTweening(bgBtn)) return;

            AudioManager.Instance.PlaySfxByTag(SfxTag.UI_Close);
            InitiateCloseAnimation();
        }

        private void InitiateCloseAnimation()
        {
            var rect = gameObject.GetComponent<RectTransform>();
            rect.DOAnchorPosY(-rect.sizeDelta.y / 2f, 0.3f)
                .SetEase(Ease.InExpo);
            rect.DOSizeDelta(new Vector2(rect.sizeDelta.x, 0), 0.3f)
                .SetEase(Ease.InExpo);
            bgBtn.DOFade(0, 0.4f)
                .OnComplete(() => { gameObject.SetActive(false); });
            petInfoPanelManager.HidePanel();
        }

#if UNITY_EDITOR
        [Button]
        private void SetDrawer()
        {
            ClearDrawer();
            var petDatas = petManager.GetPetDatas();
            var count = petDatas.Count;
            for (var i = 0; i < count; i++) CreateAndInitializeItem(petDatas.ElementAt(i).Value, i);

            UpdateContentsSize(count);
        }

        private void ClearDrawer()
        {
            foreach (var item in drawerItems)
            {
                if (item.Value != null)
                    item.Value.gameObject.SetActive(false);
                Destroy(item.Value.gameObject);
            }

            drawerItems = new Dictionary<PetType, PetInventoryItem>();
        }

        private void CreateAndInitializeItem(PetConfig config, int index)
        {
            var petController = config.obj.GetComponent<PetObject>();
            var item = Instantiate(petInventoryItemPrefab, drawerItemHolder);

            var x = index % 4;
            var y = (index - x) / 4;
            var relativeSize =
                petController.spriteRenderer.gameObject.transform.localScale.x *
                SizeFactor * 300f;
            var relativePosY = petController.spriteRenderer.gameObject.transform
                                   .localPosition.y *
                               PosFactor;

            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(Width * -1.5f + Width * x,
                -Height * y + Height / 2f + StartHeight);
            item.InitializePetInventoryItem(config.type, config.image, config.type.ToString(), Mathf.Abs(relativeSize),
                relativePosY);

            drawerItems.Add(config.type, item);
        }

        private void UpdateContentsSize(int count)
        {
            var contentsHeight = ((count - count % 4) / 4 + 1) * Height + HeightOffset;
            if (count % 4 == 0) contentsHeight -= Height;
            contents.sizeDelta = new Vector2(contents.sizeDelta.x, contentsHeight);
        }
#endif
    }
}