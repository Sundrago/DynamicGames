using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS && !UNITY_EDITOR
using Firebase.Analytics;
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.UI;
using MyUtility;

public class Ads : MonoBehaviour
{
    public static Ads Instance;
    [SerializeField] private Image TVICon;
    [SerializeField] private Sprite off, on;
    private int adCount = 0;

    public delegate void Callback();
    private Callback callbackReward;
    private Callback callbackRewardFailed;
    [SerializeField] private SFXCTRL sfx;

    
    private void Awake()
    {
        Instance = this;
#if UNITY_IOS && !UNITY_EDITOR
        // Check the user's consent status.
        // If the status is undetermined, display the request request:
        if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

    void OnApplicationPause(bool isPaused) {                 
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("debugMode", 0) == 1)
        {
            IronSource.Agent.setMetaData("is_test_suite", "enable"); 
        }
        
        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.init ("1c563f2d5", IronSourceAdUnits.REWARDED_VIDEO);
        
        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        
        DateTime today = System.DateTime.Now;
        string adDateString = PlayerPrefs.GetString("adDate", Converter.DateTimeToString(DateTime.Now.AddDays(-1)));
        adCount = PlayerPrefs.GetInt("adCount", 0);
        DateTime adDate = Converter.StringToDateTime(adDateString);
        if (today.Date != adDate.Date)
        {
            //new day
            adCount = 0;
            PlayerPrefs.SetInt("adCount", adCount);
            PlayerPrefs.SetString("adDate", Converter.DateTimeToString(DateTime.Now));
        }
        
        TVICon.sprite = adCount >= 3 ? off : on;
    }
    
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo){
        
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo){
        AudioCtrl.Instance.UnPauseBgm();

        sfx.UnPauseBGM();
        if(callbackReward!=null)
            callbackReward();
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo){
        PopupTextManager.Instance.ShowOKPopup("Failed to load AD network : " + error.ToString(), new PopupTextManager.Callback(callbackRewardFailed));
        sfx.UnPauseBGM();

        if(callbackRewardFailed!=null)
            callbackRewardFailed();
    }

    public void ShowAds(Callback _OnReward = null, Callback _OnADFailed = null, string note = null)
    {
        print("ShowAds : " + note);
#if UNITY_IOS && !UNITY_EDITOR
        FirebaseAnalytics.LogEvent("Ads", "AdType", note);
#endif
        callbackReward = _OnReward;
        callbackRewardFailed = _OnADFailed;
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo("YOUR_PLACEMENT_NAME");
            AudioCtrl.Instance.PauseBGM();
            sfx.PauseBGM();
        }
        else
        {
            PopupTextManager.Instance.ShowOKPopup("Reward Video not available. Try again later.", new PopupTextManager.Callback(callbackRewardFailed));
        }
    }

    public void WatchAdsBtnClicked()
    {
        if (adCount >= 3)
        {
            PopupTextManager.Instance.ShowOKPopup("[AdsCountExceed]");
            return;
        }
        
        TVICon.sprite = adCount >= 3 ? off : on;
        PlayerPrefs.SetInt("adCount",adCount);
        
        string output = Localize.GetLocalizedString("[watchAds]") + " (" + adCount + "/3)";
        PopupTextManager.Instance.ShowYesNoPopup(output, () =>
        {
            ShowAds(DailyTicketRewards, null, "dailyTicket");
        });
    }

    public void DailyTicketRewards()
    {
        adCount += 1;
        TVICon.sprite = adCount >= 3 ? off : on;
        PlayerPrefs.SetInt("adCount",adCount);
        PopupTextManager.Instance.ShowOKPopup("[watchedAds]티켓 10장을 받으세요!", () => { MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Ticket, Vector3.zero, 10);});
    }
}
