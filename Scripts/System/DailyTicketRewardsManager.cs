using System;
using DynamicGames.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.System
{
    /// <summary>
    ///     Manages the daily ticket rewards for watching ads.
    /// </summary>
    public class DailyTicketRewardsManager : MonoBehaviour
    {
        private const int MaxAdCount = 3;

        [Header("UI Elements")] [SerializeField]
        private Image TVIcon;

        [SerializeField] private Sprite off, on;
        private int adCount;

        public void Init()
        {
            UpdateAdCountAndDate();
            UpdateIconBasedOnAdCount();
        }

        private void UpdateAdCountAndDate()
        {
            var today = DateTime.Now;
            var adDateString = PlayerData.GetString(DataKey.adDate, Converter.DateTimeToString(today.AddDays(-1)));
            adCount = PlayerData.GetInt(DataKey.adCount);
            var adDate = Converter.StringToDateTime(adDateString);
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

        private void UpdateIconBasedOnAdCount()
        {
            TVIcon.sprite = adCount >= MaxAdCount ? off : on;
        }

        public void WatchAdsBtnClicked()
        {
            if (adCount >= 3)
            {
                PopupTextManager.Instance.ShowOKPopup("[AdsCountExceed]");
                return;
            }

            TVIcon.sprite = adCount >= 3 ? off : on;
            PlayerData.SetInt(DataKey.adCount, adCount);

            var output = Localize.GetLocalizedString("[watchAds]") + " (" + adCount + "/3)";
            PopupTextManager.Instance.ShowYesNoPopup(output,
                () => { ADManager.Instance.ShowAds(DailyTicketRewards, null, "dailyTicket"); });
        }

        public void DailyTicketRewards()
        {
            adCount += 1;
            TVIcon.sprite = adCount >= 3 ? off : on;
            PlayerData.SetInt(DataKey.adCount, adCount);
            PopupTextManager.Instance.ShowOKPopup("[watchedAds]",
                () => { MoneyManager.Instance.Reward2DAnimation(MoneyManager.RewardType.Ticket, Vector3.zero, 10); });
        }
    }
}