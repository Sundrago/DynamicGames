using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// Controls the size of an island by player device type.
    /// </summary>
    public class IslandSizeController : MonoBehaviour
    {
        [SerializeField] private RectTransform i14Pro, i14ProMax, i13Pro, i13ProMax, i12ProMax, i12Pro12, i11, iXSMax, iXSXRX, i15ProMax;
        [SerializeField] private Image[] faceImgs;
        [SerializeField] public RectTransform smallsized;
        
        private IDictionary<DeviceGeneration, RectTransform> deviceToRectTransform;
        private RectTransform rect;
        private string modelID;

        private void Awake()
        {
            deviceToRectTransform = new Dictionary<DeviceGeneration, RectTransform>
            {
                { DeviceGeneration.iPhoneX, iXSXRX },
                { DeviceGeneration.iPhoneXR, iXSXRX },
                { DeviceGeneration.iPhoneXS, iXSXRX },
                { DeviceGeneration.iPhoneXSMax, iXSMax },
                { DeviceGeneration.iPhone11, i11 },
                { DeviceGeneration.iPhone11Pro, i12Pro12 },
                { DeviceGeneration.iPhone11ProMax, i12ProMax },
                { DeviceGeneration.iPhone12, i12Pro12 },
                { DeviceGeneration.iPhone12Mini, i12Pro12 },
                { DeviceGeneration.iPhone12Pro, i12Pro12 },
                { DeviceGeneration.iPhone12ProMax, i12ProMax },
                { DeviceGeneration.iPhone13, i12Pro12 },
                { DeviceGeneration.iPhone13Mini, i12Pro12 },
                { DeviceGeneration.iPhone13Pro, i13Pro },
                { DeviceGeneration.iPhone13ProMax, i13ProMax }
            };

            if (deviceToRectTransform.TryGetValue(Device.generation, out var size))
            {
                smallsized = size;
            }
            else
            {
                if (modelID == null)
                {
                    var modelID = SystemInfo.deviceModel;
                    Debug.Log($"User Device : {modelID}");
                }
                /*
                  * iPhone15,4 : iPhone 15
                    iPhone15,5 : iPhone 15 Plus
                    iPhone16,1 : iPhone 15 Pro
                    iPhone16,2 : iPhone 15 Pro Max
                  */
                if (modelID == "iPhone14,7") smallsized = i12Pro12;
                else if (modelID == "iPhone14,8") smallsized = i12Pro12;
                else if (modelID == "iPhone15,2") smallsized = i14Pro;
                else if (modelID == "iPhone15,3") smallsized = i14ProMax;
                else if (modelID == "iPhone16,1") smallsized = i14Pro;
                else if (modelID == "iPhone16,2") smallsized = i15ProMax;
                else smallsized = i14Pro;
            }
        }

        public void Start()
        {
            rect = GetComponent<RectTransform>();

            foreach (var img in faceImgs) img.DOFade(0f, 0f);

            rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y);
            var pos = rect.anchoredPosition;
            pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
            rect.anchoredPosition = pos;
        }


        public void OpenIsland()
        {
            rect.DOSizeDelta(new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.x), 1f)
                .SetEase(Ease.OutExpo)
                .OnUpdate(() =>
                {
                    var pos = rect.anchoredPosition;
                    pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
                    rect.anchoredPosition = pos;
                });

            foreach (var img in faceImgs) img.DOFade(1f, 1f).SetEase(Ease.OutExpo);
        }

        public void CloseIsland()
        {
            rect.DOSizeDelta(new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y), 1.5f)
                .SetEase(Ease.OutExpo)
                .OnUpdate(() =>
                {
                    var pos = rect.anchoredPosition;
                    pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
                    rect.anchoredPosition = pos;
                });

            foreach (var img in faceImgs) img.DOFade(0f, 1f).SetEase(Ease.OutExpo);
        }

#if UNITY_EDITOR
        [Button]
        private void OpenTest()
        {
            rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.x);
            var pos = rect.anchoredPosition;
            pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
            rect.anchoredPosition = pos;
        }

        [Button]
        private void CloseTest()
        {
            rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y);
            var pos = rect.anchoredPosition;
            pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
            rect.anchoredPosition = pos;
        }
# endif
    }
}