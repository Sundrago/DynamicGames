using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class IslandSizeCtrl : MonoBehaviour
{
    [SerializeField] RectTransform i14Pro, i14ProMax, i13Pro, i13ProMax, i12ProMax, i12Pro12, i11, iXSMax, iXSXRX;
    [SerializeField] Image[] faceImgs;

    [SerializeField] private RectTransform smallsized;
    private RectTransform rect;

    private void Awake()
    {
        if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX)
        {
            smallsized = iXSXRX;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR)
        {
            smallsized = iXSXRX;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS)
        {
            smallsized = iXSXRX;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax)
        {
            smallsized = iXSMax;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11)
        {
            smallsized = i11;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11Pro)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11ProMax)
        {
            smallsized = i12ProMax;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12Mini)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12Pro)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12ProMax)
        {
            smallsized = i12ProMax;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13Mini)
        {
            smallsized = i12Pro12;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13Pro)
        {
            smallsized = i13Pro;
        }
        else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13ProMax)
        {
            smallsized = i13ProMax;
        }
        else
        {
            string modelID = SystemInfo.deviceModel;
            print(modelID);

            if(modelID == "iPhone14,7") smallsized = i12Pro12;
            else if (modelID == "iPhone14,8") smallsized = i12Pro12;
            else if (modelID == "iPhone15,2") smallsized = i14Pro;
            else if (modelID == "iPhone15,3") smallsized = i14ProMax;
            else smallsized = i14Pro;
        }
    }

    public void Start()
    {
        rect = GetComponent<RectTransform>();

        foreach (Image img in faceImgs)
        {
            img.DOFade(0f, 0f);
        }

        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y);
        Vector2 pos = rect.anchoredPosition;
        pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
        rect.anchoredPosition = pos;
    }

    
    public void OpenIsland()
    {
        rect.DOSizeDelta(new Vector2(smallsized.sizeDelta.x, i14Pro.sizeDelta.x), 1.5f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(()=> {
                Vector2 pos = rect.anchoredPosition;
                pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
                rect.anchoredPosition = pos;
            });

        foreach(Image img in faceImgs)
        {
            img.DOFade(1f, 1f).SetEase(Ease.OutExpo);
        }
    }

    public void CloseIsland()
    {
        rect.DOSizeDelta(new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y), 1.5f)
            .SetEase(Ease.OutExpo)
            .OnUpdate(() => {
                Vector2 pos = rect.anchoredPosition;
                pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
                rect.anchoredPosition = pos;
            });

        foreach (Image img in faceImgs)
        {
            img.DOFade(0f, 1f).SetEase(Ease.OutExpo);
        }
    }

#if UNITY_EDITOR
    [Button]
    private void OpenTest()
    {
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, i14Pro.sizeDelta.x);
        Vector2 pos = rect.anchoredPosition;
        pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
        rect.anchoredPosition = pos;
    }

    [Button]
    private void CloseTest()
    {
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(smallsized.sizeDelta.x, smallsized.sizeDelta.y);
        Vector2 pos = rect.anchoredPosition;
        pos.y = rect.sizeDelta.y * -1f / 2f + smallsized.anchoredPosition.y + smallsized.sizeDelta.y / 2f;
        rect.anchoredPosition = pos;
    }

# endif
}
