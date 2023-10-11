using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Gacha_TicketsHolder : MonoBehaviour
{
    [SerializeField]
    private Image ticket_prefab;
    [SerializeField]
    private List<GameObject> tickets;
    [SerializeField]
    private float startY, height;

    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private AudioCtrl audioCtrl;
    private int count;
    [SerializeField]
    private MoneyManager money;
    [SerializeField]
    private EndScoreCtrl endScore;

    private ticketStatus status = ticketStatus.idle;
    enum ticketStatus
    {
        idle,
        printing,
        waiting,
        reaping,
    }

    public void InitTickets(int score, int previouseHighScore, GameType gameType)
    {
        int maxScore = PlayerPrefs.GetInt("maxScore_" + gameType);
        int midScore = PlayerPrefs.GetInt("midScore_" + gameType);

        int ticketCount = 0;
        
        //social score
        if (midScore != 0 && score >= midScore) ticketCount += 1;
        if (maxScore != 0 && score >= maxScore) ticketCount += 1;
        
        //local score
        if(score >= previouseHighScore * 0.25f) ticketCount += 1;
        if(score >= previouseHighScore * 0.5f) ticketCount += 1;
        if(score >= previouseHighScore * 0.75f) ticketCount += 1;
        if (score >= previouseHighScore) ticketCount += 1;
        
        //bonus
        if(PlayerPrefs.GetInt("totalTicketCount") < 10 && score >= midScore/2f) ticketCount += 1;
        if(PlayerPrefs.GetInt("totalTicketCount") < 50 && score >= midScore) ticketCount += 1;

        switch (ticketCount)
        {
            case 0:
                //
                endScore.TicketAnimFinished();
                break;
            case 1:
                TicketAnim(1);
                break;
            case 2:
                TicketAnim(2);
                break;
            case 3:
                TicketAnim(3);
                break;
            case 4:
                TicketAnim(4);
                break;
            case 5:
                TicketAnim(6);
                break;
            default:
                TicketAnim(12);
                break;
        }
    }
    
    private void CreateTicektsObj(int count)
    {
        //reset
        status = ticketStatus.idle;
        
        for (int i = tickets.Count - 1; i >= 0; i--)
        {
            Destroy(tickets[i].gameObject);
            tickets.RemoveAt(i);
        }
        
        for (int i = 0; i < count; i++)
        {
            GameObject ticket = Instantiate(ticket_prefab.gameObject, rect);
            ticket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startY + height * i);
            ticket.SetActive(true);
            tickets.Add(ticket);
        }
    }

    public void TicketAnim(int _count)
    {
        gameObject.SetActive(true);
        count = _count;
        CreateTicektsObj(count);
        rect.anchoredPosition = new Vector2(0, GetPosY(0));

        status = ticketStatus.printing;
        switch (count)
        {
            case 1:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket1);
                rect.DOAnchorPosY(GetPosY(1), 0.5f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetEase(Ease.OutExpo)
                    .OnComplete(() => {
                        TicketAnimFinished();
                    });
                break;
            case 2:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket2);
                rect.DOAnchorPosY(GetPosY(1), 0.65f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetEase(Ease.OutBack);
                rect.DOAnchorPosY(GetPosY(2), 0.65f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.65f)
                    .SetEase(Ease.InOutQuart)
                    .OnComplete(() => {
                        TicketAnimFinished();
                    });
                break;
            case 3:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket3);
                rect.DOAnchorPosY(GetPosY(1), 0.4f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetEase(Ease.OutExpo);
                rect.DOAnchorPosY(GetPosY(2), 0.4f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.7f)
                    .SetEase(Ease.OutExpo);
                rect.DOAnchorPosY(GetPosY(3), 0.4f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(1.3f)
                    .SetEase(Ease.OutExpo)
                    .OnComplete(() => {
                        TicketAnimFinished();
                    });
                break;
            case 4:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket4);
                rect.DOAnchorPosY(GetPosY(1), 0.2f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetEase(Ease.OutQuart);
                rect.DOAnchorPosY(GetPosY(2), 0.3f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.2f)
                    .SetEase(Ease.OutQuart);
                rect.DOAnchorPosY(GetPosY(3), 0.3f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.5f)
                    .SetEase(Ease.OutQuart);
                rect.DOAnchorPosY(GetPosY(4), 0.3f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.8f)
                    .SetEase(Ease.OutQuart)
                    .OnComplete(() => {
                        TicketAnimFinished();
                    });
                break;
            case 6:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket6);
                rect.DOAnchorPosY(GetPosY(1), 0.45f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetEase(Ease.InOutQuart);
                rect.DOAnchorPosY(GetPosY(2), 0.45f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.45f)
                    .SetEase(Ease.InOutQuart);
                rect.DOAnchorPosY(GetPosY(3), 0.25f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(0.95f)
                    .SetEase(Ease.InOutCirc);
                rect.DOAnchorPosY(GetPosY(4), 0.25f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(1.2f)
                    .SetEase(Ease.InOutCirc);
                rect.DOAnchorPosY(GetPosY(5), 0.25f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(1.45f)
                    .SetEase(Ease.InOutCirc);
                rect.DOAnchorPosY(GetPosY(6), 0.25f)
                    .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                    .SetDelay(1.7f)
                    .SetEase(Ease.InOutCirc)
                    .OnComplete(() => {
                        TicketAnimFinished();
                    });
                break;
            case 12:
                audioCtrl.PlaySFXbyTag(SFX_tag.ticket12);
                for (int i = 0; i < 12; i++)
                {
                    rect.DOAnchorPosY(GetPosY(i+1), 0.19f)
                        .OnStart(()=>{audioCtrl.PlaySFXbyTag(SFX_tag.ticketGen);})
                        .SetDelay(0.19f * i)
                        .SetEase(Ease.InOutQuint);
                }
                DOVirtual.DelayedCall(2.2f, TicketAnimFinished);
                break;
            default:
                TicketAnimFinished();
                break;
        }
    }

    private void TicketAnimFinished()
    {
        status = ticketStatus.waiting;
    }


    [Button]
    public void CollectTicketBtnClicked()
    {
        if (status != ticketStatus.waiting)
        {
            DOVirtual.DelayedCall(2f, TicketAnimFinished);
            return;
        }
        status = ticketStatus.reaping;
        
        audioCtrl.PlaySFXbyTag(SFX_tag.reap);
        rect.DOAnchorPosY(GetPosY(count + 1), 0.25f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => {
                endScore.TicketAnimFinished();
                rect.DOAnchorPosY(GetPosY(count + 8), 0.75f)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() => {
                        money.AddTicket(MoneyManager.RewardType.Ticket , count);
                        CreateTicektsObj(0);
                        gameObject.SetActive(false);
                        status = ticketStatus.idle;
                    });
            });
    }
    
    float GetPosY(int idx)
    {
        float posY = (count - idx) * height * -1f;
        return posY;
        //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
    }

}
