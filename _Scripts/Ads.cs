using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyUtility;
using Unity.Advertisement.IosSupport;

public class Ads : MonoBehaviour
{
    public static Ads Instance;
    [SerializeField] private Image TVICon;
    [SerializeField] private Sprite off, on;
    private int adCount = 0;
    
    
    public enum AdsType
    {
        ticket,
    }

    private AdsType type;
    
    private void Awake()
    {
        Instance = this;
    #if UNITY_IOS
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
        IronSource.Agent.shouldTrackNetworkState (true);

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

// The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo){
        
    }
// The user completed to watch the video, and should be rewarded.
// The placement parameter will include the reward data.
// When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo){
        AudioCtrl.Instance.UnPauseBgm();
        switch (type)
        {
            case AdsType.ticket:
                PopupTextManager.Instance.ShowOKPopup("[watchedAds]티켓 10장을 받으세요!", () => { MoneyManager.Instance.Coin2DAnim(MoneyManager.RewardType.Ticket, Vector3.zero, 10);});
                break;
        }
    }
// The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo){
    }

    public void ShowAds(AdsType _type)
    {
        print("ShowAds : " + _type);
        type = _type;
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo("YOUR_PLACEMENT_NAME");
            AudioCtrl.Instance.PauseBGM();
        }
        else
        {
            
        }
    }

    public void WatchAdsBtnClicked()
    {
        if (adCount >= 3)
        {
            PopupTextManager.Instance.ShowOKPopup("[AdsCountExceed]");
            return;
        }
        
        string output = Localize.GetLocalizedString("[watchAds]") + " (" + adCount + "/3)";
        PopupTextManager.Instance.ShowYesNoPopup(output, () =>
        {
            adCount += 1;
            TVICon.sprite = adCount >= 3 ? off : on;
            PlayerPrefs.SetInt("adCount",adCount);
            ShowAds(AdsType.ticket);
        });
    }
}
