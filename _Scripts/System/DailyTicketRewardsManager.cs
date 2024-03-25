using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyUtility;

public class DailyTicketRewardsManager : MonoBehaviour
{
    [SerializeField] private Image TVICon;
    [SerializeField] private Sprite off, on;
    private int adCount = 0;

    public void Init()
    {
        UpdateAdCountAndDate();
        UpdateTVICon();
    }
    
    private void UpdateAdCountAndDate()
    {
        DateTime today = DateTime.Now;
        string adDateString = PlayerData.GetString(DataKey.adDate, Converter.DateTimeToString(today.AddDays(-1)));
        adCount = PlayerData.GetInt(DataKey.adCount, 0);
        DateTime adDate = Converter.StringToDateTime(adDateString);
        if (today.Date != adDate.Date)
        {
            ResetAdCount();
            PlayerData.SetString(DataKey.adDate, Converter.DateTimeToString(today));
        }
    }

    private void ResetAdCount()
    {
        adCount = 0;
        PlayerData.SetInt(DataKey.adCount, adCount);
#if !UNITY_EDITOR
    FirebaseAnalytics.LogEvent("Ads", "DailyAdsCount", adCount);
#endif
    }

    private void UpdateTVICon()
    {
        TVICon.sprite = adCount >= 3 ? off : on;
    }
    
    public void WatchAdsBtnClicked()
    {
        if (adCount >= 3)
        {
            PopupTextManager.Instance.ShowOKPopup("[AdsCountExceed]");
            return;
        }

        TVICon.sprite = adCount >= 3 ? off : on;
        PlayerData.SetInt(DataKey.adCount, adCount);

        string output = Localize.GetLocalizedString("[watchAds]") + " (" + adCount + "/3)";
        PopupTextManager.Instance.ShowYesNoPopup(output, () => { ADManager.Instance.ShowAds(DailyTicketRewards, null, "dailyTicket"); });
    }

    public void DailyTicketRewards()
    {
        adCount += 1;
        TVICon.sprite = adCount >= 3 ? off : on;
        PlayerData.SetInt(DataKey.adCount, adCount);
        PopupTextManager.Instance.ShowOKPopup("[watchedAds]",
            () => { MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Ticket, Vector3.zero, 10); });
    }
}
