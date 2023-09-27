using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Random = System.Random;

public class CapsuleAnimation : MonoBehaviour
{
    [SerializeField]
    private PetDrawer petDrawer;
    [SerializeField]
    private Transform top, btm1, btm2, itemObj;
    [SerializeField]
    private Image item, item_white;
    [SerializeField]
    private List<Image> capsuleImages;
    [SerializeField]
    private List<ParticleImage> particleImages;
    [SerializeField]
    private GameObject isNewFx, isOldFx;
    private List<float> particleRateOverTimes;

    [SerializeField]
    private float sizeFactor, posFactor;
    private Transform targetItemPos;
    private Vector3 midPos;
    
    enum CapsuleStatus {ready, opened, inactive}

    private CapsuleStatus status;
    private PetType type;
    private bool isNew;

    private bool isAnimPlaying = false;
    [SerializeField]
    private GachaponManager gachaponManager;
    
    private void UpdateItemImage()
    {
        type = (PetType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PetType)).Length);
        Petdata data = PetManager.Instance.GetPetDataByType(type);
        float relativePosY = data.obj.GetComponent<Pet>().spriteRenderer.gameObject.transform.localPosition.y  * posFactor;
        item.sprite = data.image;
        item.transform.localPosition = new Vector2(0, relativePosY);
        item.transform.localScale = data.obj.GetComponent<Pet>().spriteRenderer.gameObject.transform.localScale.y * Vector3.one;
        isNew = (PetManager.Instance.GetPetCountByType(type) == 0);
    }
    private void Start()
    {
        particleRateOverTimes = new List<float>();
        foreach (ParticleImage particle in particleImages)
        {
            particleRateOverTimes.Add(particle.rateOverTime);
            particle.gameObject.SetActive(false);
        }
        
        gameObject.SetActive(false);
    }
    private void StartParticles()
    {
        for(int i = 0; i < particleImages.Count; i++ )
        {
            particleImages[i].rateOverTime = particleRateOverTimes[i];
            particleImages[i].gameObject.SetActive(true);
        }
        isNewFx.SetActive(isNew);
        isOldFx.SetActive(!isNew);
    }
    
    private void PauseParticles()
    {
        for(int i = 0; i < particleImages.Count; i++ )
        {
            particleImages[i].rateOverTime = 0;
            particleImages[i].gameObject.SetActive(true);
        }
    }

    [Button]
    public void ReadyAnim()
    {
        isAnimPlaying = true;
        status = CapsuleStatus.ready;
        UpdateItemImage();
        gameObject.SetActive(true);
        foreach (Image img in capsuleImages)
            img.color = Color.white;

        itemObj.localScale = Vector3.one;
        itemObj.localPosition = Vector3.zero;
        
        item.transform.localScale = Vector3.one;
        item_white.transform.localScale = Vector3.one;
        gameObject.transform.position = Vector3.zero;
        top.position = Vector3.zero;
        btm1.position = Vector3.zero;
        btm2.position = Vector3.zero;
        item.color = Color.white;
        item_white.color = new Color(1,1,1,0.3f);
        item.GetComponent<Mask>().showMaskGraphic = false;
        PauseParticles();
        isAnimPlaying = false;

        gameObject.transform.localEulerAngles = Vector3.zero;
        
        gameObject.transform.DOLocalMoveY(-2000f, 1f)
            .SetEase(Ease.OutBack)
            .From();
        gameObject.transform.DOLocalRotate(new Vector3(0,0,-30f), 1f)
            .From()
            .OnComplete(() => {
                isAnimPlaying = false;
            });
    }

    public void BtnClicked()
    {
        if(isAnimPlaying) return;
        
        switch (status)
        {
            case CapsuleStatus.ready:
                OpenAnim();
                break;
            
            case CapsuleStatus.opened:
                GetAnim();
                break;
            
            case CapsuleStatus.inactive:
                petDrawer.HidePanel();
                gachaponManager.CapsuleAnimFinished();
                gameObject.SetActive(false);
                break;
        }
    }
    
    [Button]
    private void OpenAnim()
    {
        status = CapsuleStatus.opened;
        isAnimPlaying = true;
        StartParticles();
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gachaCapsuleOpen);

        if (isNew) DOVirtual.DelayedCall(0.15f, () => {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gacha_newOpen);
        });
        
        gameObject.transform.DOShakePosition(2, new Vector3(15, 30, 1), 7, 50);
        gameObject.transform.DOShakeRotation(1.5f, new Vector3(0, 0, 3), 10, 50);
        
        top.DOLocalMoveY(500, 1f)
            .SetDelay(0.9f)
            .SetEase(Ease.OutExpo);
        btm1.DOLocalMoveY(-500, 1f)
            .SetEase(Ease.OutExpo)
            .SetDelay(0.9f);
        btm2.DOLocalMoveY(-500, 1f)
            .SetEase(Ease.OutExpo)
            .SetDelay(0.9f);

        item_white.DOFade(0.7f, 0.135f)
            .SetLoops(6, LoopType.Yoyo)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => {
                item_white.DOFade(1, 0.2f)
                    .OnComplete(() => {
                        item.GetComponent<Mask>().showMaskGraphic = true;
                    });
            });
        item.transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 0.85f);
        
        item_white.DOFade(0f, 1f)
            .SetEase(Ease.OutSine)
            .SetDelay(0.9f);

        item.transform.DOPunchScale(Vector3.one * 0.4f, 1f)
            .SetDelay(0.9f)
            .OnComplete(() => {
                isAnimPlaying = false;
            });
    }

    [Button]
    private void GetAnim()
    {
        status = CapsuleStatus.inactive;
        isAnimPlaying = true;
        PauseParticles();
        
        foreach (Image img in capsuleImages)
        {
            img.DOFade(0, 0.5f);
        }

        
        targetItemPos = petDrawer.GetItemTransformByType(type);
        midPos = Vector3.Lerp(itemObj.transform.position, targetItemPos.position, 0.5f);
        petDrawer.ShowPanel(false);
        petDrawer.SlideToItemByIdx(type);
        
        itemObj.DOMove(midPos, 0.4f)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                DOVirtual.Float(0f, 1f, 0.4f, MoveItemObj)
                    .SetEase(Ease.OutQuart);
                targetItemPos.localScale = Vector3.one;
                targetItemPos.DOPunchScale(Vector2.one * 0.5f, 1f)
                    .SetDelay(0.2f)
                    .OnComplete(() => {
                        isAnimPlaying = false;
                        BtnClicked();
                    });
                itemObj.DOScale(0.2f, 0.2f);
                itemObj.DOScale(0, 0.1f)
                    .SetDelay(0.2f)
                    .SetEase(Ease.InOutQuint);

                DOVirtual.DelayedCall(0.2f, () => {
                    if(isNew)  AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gacha_newItem);
                    else AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gacha_item);
                    PetManager.Instance.AddPetCountByType(type);
                    petDrawer.drawerItems[type].UpdateItemWithAnim();
                });
            });
    }

    private void MoveItemObj(float _normal)
    {
        itemObj.position = Vector3.Lerp(midPos, targetItemPos.position, _normal);
    }
}
