using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class UnlockBtnManager : MonoBehaviour
{
    public static UnlockBtnManager Instance;
    [SerializeField]
    private Transform ticketbtn, coinBtn;
    [SerializeField]
    private Image ticketActive, coinActive, ticket, coin, arrow;
    [SerializeField]
    private Transform ticketGlow, coinGlow;

    [SerializeField] private GameObject unlockFX;

    private bool isAnimPlaying = false;
    private GameType targetGameType;
    [SerializeField] private DragSprite targetBlockObj;

    private bool isHidden = false;

    
    private void Awake()
    {
        Instance = this;
        ticket.color = new Color(1, 1, 1, 0);
        coin.color = new Color(1, 1, 1, 0);
        ticketActive.color = new Color(1, 1, 1, 0);
        coinActive.color = new Color(1, 1, 1, 0);
        gameObject.SetActive(false);
    }

    public void SetBtnActive()
    {
        ticketbtn.GetComponent<Button>().interactable = true;
        coinBtn.GetComponent<Button>().interactable = true;
        
        int isTicketActive = (MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 100)) ? 1 : 0;
        int isCoinActive = (MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Key, 1)) ? 1 : 0;

        DOTween.Kill(arrow);
        DOTween.Kill(ticketActive);
        DOTween.Kill(coinActive);
        ticketActive.DOFade(isTicketActive, 0.3f)
            .SetDelay(0.2f);
        coinActive.DOFade(isCoinActive, 0.3f)
            .SetDelay(0.2f);

        if (isTicketActive == 1 && !DOTween.IsTweening(ticketGlow))
        {
            ticketGlow.localPosition = new Vector3(-400, 0, 0);
            ticketGlow.DOLocalMoveX(400, 1)
                .SetDelay(0.3f);
        }
        
        if (isCoinActive == 1 && !DOTween.IsTweening(coinGlow))
        {
            coinGlow.localPosition = new Vector3(-400, 0, 0);
            coinGlow.DOLocalMoveX(400, 1)
                .SetDelay(0.3f);
        }

        if (isCoinActive == 1 && !DOTween.IsTweening(coinBtn.gameObject.transform))
        {
            coinBtn.gameObject.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.5f, 1)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.OutQuad);
        }
        
        if (isCoinActive == 0 && DOTween.IsTweening(coinBtn.gameObject.transform))
        {
            DOTween.Kill(coinBtn.gameObject.transform);
            coinBtn.gameObject.transform.localScale = Vector3.one;
        }
    }

    public void TicketBtnClicked()
    {
        if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Ticket, 100))
        {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.unable);
            if (!DOTween.IsTweening(ticketbtn)) ticketbtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
            return;
        }
        
        if (MoneyManager.Instance.SubtractTicket(MoneyManager.RewardType.Ticket, 100))
        {
            MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Key, ticketbtn.transform.position, 1);
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.gotCoin);
        }
        SetBtnActive();
    }

    public void CoinBtnClicked()
    {
        if (!MoneyManager.Instance.HasEnoughTicket(MoneyManager.RewardType.Key, 1))
        {
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.unable);
            if (!DOTween.IsTweening(coinBtn)) coinBtn.DOPunchPosition(new Vector3(10, 0, 0), 0.5f);
            return;
        }
        
        if(!isAnimPlaying && MoneyManager.Instance.SubtractTicket(MoneyManager.RewardType.Key, 1)) Coin2DAnim();

        TutorialManager.Instancee.GameUnlocked();
        SetBtnActive();
        targetBlockObj.BtnClicked();
        Hide(true);
    }

    [Button]
    public void Coin2DAnim()
    {
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.sparkle);
        isAnimPlaying = true;
        float _velocity = 0.5f;
        Vector3 startPos = MoneyManager.Instance.keyHolder_ui.transform.position;
        float durationFactor = 2f;

        Vector3[] path = new Vector3[3];
        path[2] = targetBlockObj.gameObject.transform.position;
        GameObject obj = MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.Key].Get();
        float velocity = _velocity * Random.Range(0.8f, 1.2f);

        obj.transform.localScale = Vector3.one;
        obj.transform.position = startPos;

        float angle = Random.Range(0.5f, 1.5f) * Mathf.PI;
        path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
        path[0] = Vector3.Lerp(path[0], path[2], 0.4f);

        Vector3 diff = startPos - path[0];
        path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - (diff * Random.Range(0.3f, 0.8f));

        DOVirtual.DelayedCall(durationFactor * 0.6f, () => {
            obj.GetComponent<SpriteAnimator>().pauseAtIdx = 10;
        });
        
        obj.transform.DORotate(Vector3.zero, 1f);
        obj.transform.DOMove(path[0], 0.2f * durationFactor)
            .SetEase(Ease.OutCirc)
            .OnComplete(() => {
                obj.transform.DOPath(path, 0.8f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                    .SetEase((Ease.InOutCubic))
                    .OnComplete(() => {
                        MoneyManager.Instance.obj_pools[(int)MoneyManager.RewardType.Key].Release(obj);
                        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.insertCoin);
                        isAnimPlaying = false;
                        GameObject fx = Instantiate(unlockFX);
                        fx.transform.SetParent(targetBlockObj.transform, true);
                        fx.transform.localPosition = Vector3.zero;
                        fx.SetActive(true);
                        targetBlockObj.Unlock();
                        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.key);
                    });
                obj.transform.DOScale(Vector3.zero, 0.79f * durationFactor)
                    .SetEase(Ease.InQuart);
            });
    }

    public void Init(DragSprite _targetBlockObj)
    {
        print("_INIT");
        targetBlockObj = _targetBlockObj;

        DOTween.Kill(coinActive);
        gameObject.SetActive(true);
        ticket.color = new Color(1, 1, 1, 0);
        coin.color = new Color(1, 1, 1, 0);
        ticketActive.color = new Color(1, 1, 1, 0);
        coinActive.color = new Color(1, 1, 1, 0);
        
        gameObject.transform.SetParent(targetBlockObj.transform, true);
        gameObject.transform.DOLocalMove(new Vector3(5f, -12, 0), 0.5f);
        gameObject.transform.DOLocalRotate(Vector3.zero, 0.5f);

        ticket.DOFade(1, 2f);
        coin.DOFade(1, 2f);
        arrow.DOFade(1, 2f);
        SetBtnActive();
        isHidden = false;
    }

    public void Hide(bool forceHide = false)
    {
        print("hide");
        if (!forceHide)
        {
            if(isHidden) return;
            if(DOTween.IsTweening(coinActive)) return;
        }

        DOTween.Kill(ticket);
        DOTween.Kill(coin);
        DOTween.Kill(arrow);
        DOTween.Kill(coinActive);
        DOTween.Kill(ticketActive);
        
        ticket.DOFade(0, 1f);
        coin.DOFade(0, 1f);
        coinActive.DOFade(0, 1f);
        arrow.DOFade(0, 1f);
        ticketActive.DOFade(0, 1f);
        arrow.DOFade(0, 1f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        ticketbtn.GetComponent<Button>().interactable = false;
        coinBtn.GetComponent<Button>().interactable = false;
        isHidden = true;
    }
}
