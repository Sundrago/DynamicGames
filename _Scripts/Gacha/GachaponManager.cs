using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Random = UnityEngine.Random;

public class GachaponManager : MonoBehaviour
{
    [SerializeField] private Sprite[] gachapon_capsule_sprites;
    [SerializeField] private Sprite[] gachapon_capsule_fullsized_sprites;
    [SerializeField] private Image[] gacapon_caplsule_ui;
    [SerializeField] private Transform takeoutCapsule, lever, gachapon, coinInsertPos;
    [SerializeField] private AudioCtrl audio;
    [SerializeField] private DragSprite GachaponBtn;

    [SerializeField] private Image myCapsule_s, myCapsule_l, bg;
    [SerializeField]
    private TextMeshProUGUI guideText;
    [SerializeField] private CapsuleAnimation capsuleAnimation;
    [SerializeField]
    private SFXCTRL sfx;

    [SerializeField]
    private Transform ticketbtn, coinBtn;
    [SerializeField]
    private Image ticketActive, coinActive;
    [SerializeField]
    private Transform ticketGlow, coinGlow;
    
    private bool isAnimPlaying = false;

    public enum gachaponStatus { idle, insertCoin, rotateLever, closup};

    private gachaponStatus status = gachaponStatus.idle;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    [Button]
    private void RnadomizePonCapsule()
    {
        foreach (Image img in gacapon_caplsule_ui)
        {
            img.gameObject.transform.DOShakePosition(0.3f* Random.Range(0.85f, 1.15f), new Vector3(0.1f* Random.Range(0.85f, 1.15f), 0.5f* Random.Range(0.85f, 1.15f), 0), 4)
                .SetEase(Ease.InOutSine);
        }
        
        foreach (Image img in gacapon_caplsule_ui)
        {
            img.sprite = gachapon_capsule_sprites[Random.Range(0, gachapon_capsule_sprites.Length)];
            img.gameObject.transform.localEulerAngles = new Vector3(0, 0, 90 * Random.Range(-1, 2));
        }
        takeoutCapsule.localPosition = new Vector3(0, 20, 0);

        int rnd = Random.Range(0, gachapon_capsule_sprites.Length);
        myCapsule_s.sprite = gachapon_capsule_sprites[rnd];
        myCapsule_l.sprite = gachapon_capsule_fullsized_sprites[rnd];
    }
    
    [Button]
    private void RotateLever()
    {
        DOTween.Kill(lever.transform);
        lever.transform.localScale = Vector3.one * 10f;
        
        isAnimPlaying = true;
        lever.eulerAngles = Vector3.zero;
        takeoutCapsule.localPosition = new Vector3(0, 20, 0);

        audio.PlaySFXbyTag(SFX_tag.gacha_simple_bgm);
        
        audio.PlaySFXbyTag(SFX_tag.gacha_rotateLever);
        lever.DORotate(new Vector3(0, 0, -180), 0.7f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                audio.PlaySFXbyTag(SFX_tag.gacha_rotateLever);
                lever.DORotate(new Vector3(0, 0, -360), 0.7f)
                    .SetEase(Ease.InOutQuart);
            });

        DOVirtual.DelayedCall(0.7f, ShakePonCapsulesAndGet);
    }
    
    private void ShakePonCapsulesAndGet()
    {
        audio.PlaySFXbyTag(SFX_tag.gacha_capsules);
        foreach (Image img in gacapon_caplsule_ui)
        {
            img.gameObject.transform.DOShakePosition(1.5f* Random.Range(0.85f, 1.15f), new Vector3(0.1f* Random.Range(0.85f, 1.15f), 0.5f* Random.Range(0.85f, 1.15f), 0), 4)
                .SetEase(Ease.InOutSine);
        }

        DOTween.Kill(takeoutCapsule.transform);
        takeoutCapsule.DOLocalMoveY(0, 1f)
            .SetEase(Ease.OutBounce)
            .SetDelay(0.5f)
            .OnComplete(() => {
                isAnimPlaying = false;
                status = gachaponStatus.rotateLever;
                takeoutCapsule.transform.DOScale(1.05f, 0.2f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
            })
            .OnStart(() => {audio.PlaySFXbyTag(SFX_tag.gacha_drop);});
    }
 
    [Button]
    private void CapsuleCloseUp()
    {
        DOTween.Kill(takeoutCapsule.transform);
        takeoutCapsule.transform.localScale = Vector3.one;
        
        isAnimPlaying = true;
        takeoutCapsule.DOLocalMoveY(-500, 1f)
            .OnComplete(() => {
                isAnimPlaying = true;
                status = gachaponStatus.closup;
            })
            .SetEase(Ease.InOutSine);
        takeoutCapsule.DORotate(new Vector3(0, 0, 30), 0.5f);
        capsuleAnimation.ReadyAnim();
    }

    public void CapsuleAnimFinished(bool isnew)
    {
        isAnimPlaying = false;
        status = gachaponStatus.idle;
        sfx.ChangeBGMVolume(1f);
        RnadomizePonCapsule();
        TutorialManager.Instancee.TutorialF_Check();

        if(isnew)DOVirtual.DelayedCall(0.5f, HidePanel);
    }

    [Button]
    private void NotEnoughTicket()
    {
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.notEnoughMoney);
        gachapon.transform.localEulerAngles = Vector3.zero;
        gachapon.DOPunchRotation(new Vector3(0, 0, 2), 0.5f);
    }

    [Button]
    public void ShowPanel()
    {
        if (gameObject.activeSelf) return;
        if (DOTween.IsTweening(bg)) return;
        
        GachaponBtn.KillFX();

        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup3);
        SetBtnActive();

        RnadomizePonCapsule();
        gachapon.transform.position = Vector3.zero;
        gachapon.transform.eulerAngles = Vector3.zero;

        gachapon.transform.DOLocalMoveY(-2500f, 0.8f)
            .From()
            .SetEase(Ease.OutExpo);
        gachapon.transform.DORotate(new Vector3(0f, 0f, 30f), 0.8f)
            .From()
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                TutorialManager.Instancee.TutorialE_check();
            });

    bg.DOFade(0.2f, 1f);
        gameObject.SetActive(true);
    }

    [Button]
    public void HidePanel()
    {
        if (status != gachaponStatus.idle)
        {
            PonBtnClicked();
            return;
        }
        
        if(DOTween.IsTweening(bg)) return;
        
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.UI_CLOSE);
        gachapon.transform.position = Vector3.zero;
        gachapon.transform.eulerAngles = Vector3.zero;

        bg.DOFade(0, 1f);
        gachapon.transform.DOLocalMoveY(-3000f, 1f)
            .SetEase(Ease.InOutExpo)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                MainCanvas.Instance.Offall();
            });
        gachapon.transform.DORotate(new Vector3(0f,0f,10), 0.8f)
            .SetEase(Ease.OutExpo);
    }

    public void PonBtnClicked()
    {
        if(isAnimPlaying) return;
        switch (status)
        {
            case gachaponStatus.insertCoin :
                RotateLever();
                sfx.ChangeBGMVolume(0.5f);
                break;
            case gachaponStatus.rotateLever :
                CapsuleCloseUp();
                break;
            case gachaponStatus.closup :
                break;
            case gachaponStatus.idle :
                
                break;
        }
        // NotEnoughTicket();
    }

    public void SetBtnActive()
    {
        int isTicketActive = (MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 50)) ? 1 : 0;
        int isCoinActive = (MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.GachaCoin, 1)) ? 1 : 0;
        
        ticketActive.DOFade(isTicketActive, 0.3f);
        coinActive.DOFade(isCoinActive, 0.3f);

        if (isTicketActive == 1 && !DOTween.IsTweening(ticketGlow))
        {
            ticketGlow.localPosition = new Vector3(-400, 0, 0);
            ticketGlow.DOLocalMoveX(400, 1);
        }
        
        if (isCoinActive == 1 && !DOTween.IsTweening(coinGlow))
        {
            coinGlow.localPosition = new Vector3(-400, 0, 0);
            coinGlow.DOLocalMoveX(400, 1);
        }

        if (isCoinActive == 1 && !DOTween.IsTweening(coinBtn.gameObject.transform) &&!isAnimPlaying && status == gachaponStatus.idle)
        {
            coinBtn.gameObject.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.5f, 1)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.OutQuad);
        }
        
        if (isAnimPlaying || isCoinActive == 0 || status != gachaponStatus.idle)
        {
            DOTween.Kill(coinBtn.gameObject.transform);
            coinBtn.gameObject.transform.localScale = Vector3.one;
        }
        
        guideText.gameObject.SetActive(isTicketActive == 0 && isCoinActive == 0 && isAnimPlaying == false && status == gachaponStatus.idle);
    }

    public void TicketBtnClicked()
    {
        if (MoneyManager.Instance.SubtractTicket(MoneyManager.RewardType.Ticket, 50))
        {
            MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.GachaCoin, ticketbtn.transform.position, 1, 0.5f, 0);
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gotCoin);
            TutorialManager.Instancee.TicketBtnClicked();
            SetBtnActive();
            return;
        }
        
        if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 50))
        {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.notEnoughMoney);
            if (!DOTween.IsTweening(ticketbtn)) ticketbtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
        }
        SetBtnActive();
    }

    public void CoinBtnClicked()
    {
        if (!isAnimPlaying && status == gachaponStatus.idle &&
            MoneyManager.Instance.SubtractTicket(MoneyManager.RewardType.GachaCoin, 1))
        {
            Coin2DAnim();
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.popup2);
            TutorialManager.Instancee.CoinBtnClicked();
            SetBtnActive();
            return;
        }
        
        if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.GachaCoin, 1))
        {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.notEnoughMoney);
            if (!DOTween.IsTweening(coinBtn)) coinBtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
        }
        SetBtnActive();
    }

    public void Coin2DAnim()
    {
        isAnimPlaying = true;
        float _velocity = 0.5f;
        Vector3 startPos = MoneyManager.Instance.gachaCoinHolder_ui.transform.position;
        float durationFactor = 2f;

        Vector3[] path = new Vector3[3];
        path[2] = coinInsertPos.position;
        GameObject obj = MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.GachaCoin].Get();
        float velocity = _velocity * Random.Range(0.8f, 1.2f);

        obj.transform.localScale = Vector3.one;
        obj.transform.position = startPos;

        float angle = Random.Range(0.5f, 1.5f) * Mathf.PI;
        path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
        path[0] = Vector3.Lerp(path[0], path[2], 0.4f);

        Vector3 diff = startPos - path[0];
        path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - (diff * Random.Range(0.3f, 0.8f));

        DOVirtual.DelayedCall(durationFactor * 0.6f, () => {
            obj.GetComponent<SpriteAnimator>().pauseAtIdx = 3;
        });
        
        obj.transform.DORotate(Vector3.zero, 1f);
        obj.transform.DOMove(path[0], 0.2f * durationFactor)
            .SetEase(Ease.OutCirc)
            .OnComplete(() => {
                obj.transform.DOPath(path, 0.8f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                    .SetEase((Ease.InOutCubic))
                    .OnComplete(() => {
                        MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.GachaCoin].Release(obj);
                        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.insertCoin);
                        status = gachaponStatus.insertCoin;
                        lever.DOPunchScale(Vector3.one, 0.5f, 5)
                            .SetLoops(-1);
                        isAnimPlaying = false;
                    });
                obj.transform.DOScale(Vector3.zero, 0.79f * durationFactor)
                    .SetEase(Ease.InQuart);
            });
    }
}
