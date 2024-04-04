using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.MiniGames;
using DynamicGames.System;
using DynamicGames.UI;
using DynamicGames.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.Gachapon
{
    /// <summary>
    ///     Class responsible for managing tickets in a end-game-score UI.
    /// </summary>
    public class TicketsController : MonoBehaviour
    {
        [Header("Managers and Controllers")] [SerializeField]
        private AudioManager audioManager;

        [SerializeField] private GameScoreManager gameScoreManager;
        [SerializeField] private MoneyManager moneyManager;

        [Header("UI Components")] [SerializeField]
        private List<GameObject> tickets;

        [SerializeField] private RectTransform rect;
        [SerializeField] private Image ticket_prefab;
        [SerializeField] private float startY, height;

        private TicketStatus status = TicketStatus.Idle;
        private int ticketCount;

        public void InitTickets(int score, int previousHighScore, GameType gameType)
        {
            var maxScore = PlayerPrefs.GetInt("maxScore_" + gameType);
            var midScore = PlayerPrefs.GetInt("midScore_" + gameType);

            var ticketCount = CalculateSocialScore(score, maxScore, midScore);
            ticketCount += CalculateLocalScore(score, previousHighScore);
            ticketCount += CalculateBonus(score, midScore);

            PlayTicketAnimation(ticketCount);
        }

        private int CalculateSocialScore(int score, int maxScore, int midScore)
        {
            var ticketCount = 0;
            if (midScore != 0 && score >= midScore) ticketCount += 1;
            if (maxScore != 0 && score >= maxScore) ticketCount += 1;
            return ticketCount;
        }

        private int CalculateLocalScore(int score, int previousHighScore)
        {
            var ticketCount = 0;
            float[] scoreRatios = { 0.25f, 0.5f, 0.75f, 1f };
            foreach (var ratio in scoreRatios)
                if (score >= previousHighScore * ratio)
                    ticketCount += 1;

            return ticketCount;
        }

        private int CalculateBonus(int score, int midScore)
        {
            var ticketCount = 0;

            if (PlayerData.GetInt(DataKey.totalTicketCount) < 500 && score >= midScore / 2f) ticketCount += 1;
            if (PlayerData.GetInt(DataKey.totalTicketCount) < 200 && score >= midScore) ticketCount += 1;

            return ticketCount;
        }

        private void PlayTicketAnimation(int ticketCount)
        {
            switch (ticketCount)
            {
                case 0:
                    gameScoreManager.TicketAnimFinished();
                    break;
                default:
                    var animCount = ticketCount > 5 ? 12 : ticketCount == 5 ? 6 : ticketCount;
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
                    audioManager.PlaySfxByTag(SfxTag.TicketOne);
                    PlayTicketAnimation(1, 0.5f, 0, Ease.OutExpo, true);
                    break;
                case 2:
                    audioManager.PlaySfxByTag(SfxTag.TicketTwo);

                    PlayTicketAnimation(1, 0.65f, 0, Ease.OutBack);
                    PlayTicketAnimation(2, 0.65f, 0.65f, Ease.InOutQuart, true);
                    break;
                case 3:
                    audioManager.PlaySfxByTag(SfxTag.TicketThree);
                    PlayTicketAnimation(1, 0.4f, 0, Ease.OutExpo);
                    PlayTicketAnimation(2, 0.4f, 0.7f, Ease.OutExpo);
                    PlayTicketAnimation(3, 0.4f, 1.3f, Ease.OutExpo, true);
                    break;
                case 4:
                    audioManager.PlaySfxByTag(SfxTag.TicketFour);
                    PlayTicketAnimation(1, 0.2f);
                    PlayTicketAnimation(2, 0.3f, 0.2f);
                    PlayTicketAnimation(3, 0.3f, 0.5f);
                    PlayTicketAnimation(4, 0.3f, 0.8f, Ease.OutQuart, true);
                    break;
                case 6:
                    audioManager.PlaySfxByTag(SfxTag.TicketSix);
                    PlayTicketAnimation(1, 0.45f, 0, Ease.InOutQuart);
                    PlayTicketAnimation(2, 0.45f, 0.45f, Ease.InOutQuart);
                    PlayTicketAnimation(3, 0.25f, 0.95f, Ease.InOutQuart);
                    PlayTicketAnimation(4, 0.25f, 1.2f, Ease.InOutQuart);
                    PlayTicketAnimation(5, 0.25f, 1.45f, Ease.InOutQuart);
                    PlayTicketAnimation(6, 0.25f, 1.7f, Ease.InOutQuart, true);
                    break;
                case 12:
                    audioManager.PlaySfxByTag(SfxTag.TicketTwelve);
                    for (var i = 0; i < 12; i++) PlayTicketAnimation(i + 1, 0.19f, 0.19f * i, Ease.InOutQuint);

                    DOVirtual.DelayedCall(2.2f, TicketAnimFinished);
                    break;
                default:
                    float delay;
                    if (ticketCount < 10) delay = 0.18f;
                    else if (ticketCount < 20) delay = 0.15f;
                    else delay = 0.11f;
                    audioManager.PlaySfxByTag(SfxTag.TicketStart);
                    for (var i = 0; i < ticketCount; i++)
                        PlayTicketAnimation(i + 1, delay, delay * i, Ease.OutQuart, i >= ticketCount - 1);

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
            for (var i = tickets.Count - 1; i >= 0; i--) Destroy(tickets[i].gameObject);

            tickets.Clear();
        }

        private void InitializeTickets(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var ticket = Instantiate(ticket_prefab.gameObject, rect);
                ticket.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startY + height * i);
                ticket.SetActive(true);
                tickets.Add(ticket);
            }
        }

        private void PlayTicketAnimation(int index, float duration, float delay = 0f, Ease ease = Ease.OutQuart,
            bool isLast = false)
        {
            rect.DOAnchorPosY(GetPosY(index), duration)
                .OnStart(() => { audioManager.PlaySfxByTag(SfxTag.TicketGen); })
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

        private float GetPosY(int idx)
        {
            var posY = (ticketCount - idx) * height * -1f;
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

            var pos = Input.mousePosition;
            pos = Camera.main.ScreenToWorldPoint(pos);

            moneyManager.Reward2DAnimation(MoneyManager.RewardType.Ticket, pos, ticketCount);
            audioManager.PlaySfxByTag(SfxTag.TicketReap);
            CollectTicketAnimation();
        }

        private void CollectTicketAnimation()
        {
            rect.DOAnchorPosY(GetPosY(ticketCount + 1), 0.25f)
                .SetEase(Ease.OutExpo)
                .OnComplete(() =>
                {
                    gameScoreManager.TicketAnimFinished();
                    rect.DOAnchorPosY(GetPosY(ticketCount + 8), 0.75f)
                        .SetEase(Ease.InQuart)
                        .OnComplete(() =>
                        {
                            CreateTicketsObj(0);
                            gameObject.SetActive(false);
                            status = TicketStatus.Idle;
                        });
                });
        }

        private enum TicketStatus
        {
            Idle,
            Printing,
            Waiting,
            Reaping
        }
    }
}