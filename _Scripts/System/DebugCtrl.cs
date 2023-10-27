using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCtrl : MonoBehaviour
{
    [SerializeField] private Ranking_UI ranking_ui;
    [SerializeField] private LeaderboardManger leaderboardManger;
    [SerializeField] private Gacha_TicketsHolder gachaTicketsHolder;
    [SerializeField] private SFXCTRL sfx;
    [SerializeField]
    private GameObject debug_front_window;
    private int hiddenBtnClickCount = 0;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void hiddenBtnClicked()
    {
        ++hiddenBtnClickCount;
        if (hiddenBtnClickCount > 5)
        {
            gameObject.SetActive(true);
            debug_front_window.SetActive(true);
        }
    }

    public void Debug_ShowRangkingUI()
    {
        StartCoroutine(ranking_ui.ShowRankingUI((GameType)0));
    }

    public void Debug_RankingIcon()
    {
        leaderboardManger.debug_randomRank = true;
        StartCoroutine(leaderboardManger.GetDataFromServers());
    }

    public void Close()
    {
        debug_front_window.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowScore()
    {
        gachaTicketsHolder.TicketAnim(0);
        EndScoreCtrl.Instance.ShowScore(UnityEngine.Random.Range(0,300), GameType.shoot);
    }

    public void HideScore()
    {
        EndScoreCtrl.Instance.HideScore();
        sfx.PlayBGM(-1);
    }

    public void GetTickets(int amount)
    {
        gachaTicketsHolder.TicketAnim(amount);
    }

    public void AddTickets()
    {
        MoneyManager.Instance.AddTicket(MoneyManager.RewardType.Ticket, 50);
    }
    
    public void RemoveData()
    {
        PlayerPrefs.DeleteAll();
    }
}
