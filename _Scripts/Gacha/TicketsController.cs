using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyUtility;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

/// <summary>
/// Class responsible for managing tickets in a end-game-score UI.
/// </summary>
public class TicketsController : MonoBehaviour
{
    [Header("Managers and Controllers")] 
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private EndScoreCtrl endScoreController;
    [SerializeField] private MoneyManager moneyManager;

    [Header("UI Components")] 
    [SerializeField] private List<GameObject> tickets;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image ticket_prefab;
    [SerializeField] private float startY, height;

    private int ticketCount;
    private TicketStatus status = TicketStatus.Idle;
    enum TicketStatus
    {
        Idle,
        Printing,
        Waiting,
        Reaping,
    }

    public void InitTickets(int score, int previousHighScore, GameType gameType)
    {
        int maxScore = PlayerPrefs.GetInt("maxScore_" + gameType);
        int midScore = PlayerPrefs.GetInt("midScore_" + gameType);

        int ticketCount = CalculateSocialScore(score, maxScore, midScore);
        ticketCount += CalculateLocalScore(score, previousHighScore);
        ticketCount += CalculateBonus(score, midScore);

        PlayTicketAnimation(ticketCount);
    }

    private int CalculateSocialScore(int score, int maxScore, int midScore)
    {
        int ticketCount = 0;
        if (midScore != 0 && score >= midScore) ticketCount += 1;
        if (maxScore != 0 && score >= maxScore) ticketCount += 1;
        return ticketCount;
    }

    private int CalculateLocalScore(int score, int previousHighScore)
    {
        int ticketCount = 0;
        float[] scoreRatios = { 0.25f, 0.5f, 0.75f, 1f };
        foreach (var ratio in scoreRatios)
        {
            if (score >= previousHighScore * ratio) ticketCount += 1;
        }

        return ticketCount;
    }

    private int CalculateBonus(int score, int midScore)
    {
        int ticketCount = 0;

        if (PlayerData.GetInt(DataKey.totalTicketCount) < 500 && score >= midScore / 2f) ticketCount += 1;
        if (PlayerData.GetInt(DataKey.totalTicketCount) < 200 && score >= midScore) ticketCount += 1;

        return ticketCount;
    }

    private void PlayTicketAnimation(int ticketCount)
    {
        switch (ticketCount)
        {
            case 0:
                endScoreController.TicketAnimFinished();
                break;
            default:
                int animCount = ticketCount > 5 ? 12 : (ticketCount == 5 ? 6 : ticketCount);
                TicketAnim(animCount);
                break;
        }
    }
    
    public void TicketAnim(int _count)
    {
        gameObject.SetActive(true);
        ticketCount = _count;
        CreateTicketsObj(ticketCount);
        rect.anchoredPosition = new Vector2(0, GetPosY(0));

        status = TicketStatus.Printing;
        switch (ticketCount)
        {
            case 1:
                audioManager.PlaySFXbyTag(SfxTag.ticket1);
                PlayTicketAnimation(1, 0.5f, 0, Ease.OutExpo, true);
                break;
            case 2:
                audioManager.PlaySFXbyTag(SfxTag.ticket2);
                
                PlayTicketAnimation(1, 0.65f, 0, Ease.OutBack);
                PlayTicketAnimation(2, 0.65f, 0.65f, Ease.InOutQuart, true);
                break;
            case 3:
                audioManager.PlaySFXbyTag(SfxTag.ticket3);
                PlayTicketAnimation(1, 0.4f, 0, Ease.OutExpo);
                PlayTicketAnimation(2, 0.4f, 0.7f, Ease.OutExpo);
                PlayTicketAnimation(3, 0.4f, 1.3f, Ease.OutExpo, true);
                break;
            case 4:
                audioManager.PlaySFXbyTag(SfxTag.ticket4);
                PlayTicketAnimation(1, 0.2f, 0, Ease.OutQuart);
                PlayTicketAnimation(2, 0.3f, 0.2f, Ease.OutQuart);
                PlayTicketAnimation(3, 0.3f, 0.5f, Ease.OutQuart);
                PlayTicketAnimation(4, 0.3f, 0.8f, Ease.OutQuart, true);
                break;
            case 6:
                audioManager.PlaySFXbyTag(SfxTag.ticket6);
                PlayTicketAnimation(1, 0.45f, 0, Ease.InOutQuart);
                PlayTicketAnimation(2, 0.45f, 0.45f, Ease.InOutQuart);
                PlayTicketAnimation(3, 0.25f, 0.95f, Ease.InOutQuart);
                PlayTicketAnimation(4, 0.25f, 1.2f, Ease.InOutQuart);
                PlayTicketAnimation(5, 0.25f, 1.45f, Ease.InOutQuart);
                PlayTicketAnimation(6, 0.25f, 1.7f, Ease.InOutQuart, true);
                break;
            case 12:
                audioManager.PlaySFXbyTag(SfxTag.ticket12);
                for (int i = 0; i < 12; i++)
                {
                    PlayTicketAnimation(i+1, 0.19f, 0.19f * i, Ease.InOutQuint);
                }

                DOVirtual.DelayedCall(2.2f, TicketAnimFinished);
                break;
            default:
                float delay;
                if (ticketCount < 10) delay = 0.18f;
                else if (ticketCount < 20) delay = 0.15f;
                else delay = 0.11f;
                audioManager.PlaySFXbyTag(SfxTag.ticketStart);
                for (int i = 0; i < ticketCount; i++)
                {
                    PlayTicketAnimation(i + 1, delay, delay * i, Ease.OutQuart, i >= ticketCount - 1);
                }

                break;
        }
    }

    private void CreateTicketsObj(int count)
    {
        ResetTickets();
        InitializeTickets(count);
    }

    private void ResetTickets()
    {
        status = TicketStatus.Idle;
        for (int i = tickets.Count - 1; i >= 0; i--)
        {
            Destroy(tickets[i].gameObject);
        }

        tickets.Clear();
    }

    private void InitializeTickets(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject ticket = Instantiate(ticket_prefab.gameObject, rect);
            ticket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startY + height * i);
            ticket.SetActive(true);
            tickets.Add(ticket);
        }
    }

    private void PlayTicketAnimation(int index, float duration, float delay = 0f, Ease ease = Ease.OutQuart,
        bool isLast = false)
    {
        rect.DOAnchorPosY(GetPosY(index), duration)
            .OnStart(() => { audioManager.PlaySFXbyTag(SfxTag.ticketGen); })
            .SetDelay(delay)
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (isLast) TicketAnimFinished();
            });
    }

    private void TicketAnimFinished()
    {
        status = TicketStatus.Waiting;
    }

    float GetPosY(int idx)
    {
        float posY = (ticketCount - idx) * height * -1f;
        return posY;
    }

    public void CollectTicketBtnClicked()
    {
        if (status != TicketStatus.Waiting)
        {
            DOVirtual.DelayedCall(2f, TicketAnimFinished);
            return;
        }
        if (DOTween.IsTweening(rect)) return;

        status = TicketStatus.Reaping;
        
        Vector3 pos = Input.mousePosition;
        pos = Camera.main.ScreenToWorldPoint(pos);
        
        moneyManager.Coin2DAnim(MoneyManager.RewardType.Ticket, pos, ticketCount);
        audioManager.PlaySFXbyTag(SfxTag.reap);
        CollectTicketAnimation();
    }

    private void CollectTicketAnimation()
    {
        rect.DOAnchorPosY(GetPosY(ticketCount + 1), 0.25f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                endScoreController.TicketAnimFinished();
                rect.DOAnchorPosY(GetPosY(ticketCount + 8), 0.75f)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() =>
                    {
                        // money.AddTicket(MoneyManager.RewardType.Ticket , count);
                        CreateTicketsObj(0);
                        gameObject.SetActive(false);
                        status = TicketStatus.Idle;
                    });
            });
    }
}