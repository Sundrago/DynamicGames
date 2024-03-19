using System;
using UnityEngine;
using PrivateKeys;
using MyUtility;

#if !UNITY_EDITOR
using Firebase.Analytics;
#endif
#if UNITY_IPHONE
using Unity.Advertisement.IosSupport;
#endif

public class ADManager : MonoBehaviour
{
    public static ADManager Instance { get; private set; }
    public event Action OnAdReward;
    public event Action OnAdFailed;

    [SerializeField] private SFXCTRL sfxctrl;
    [SerializeField] private DailyTicketRewardsManager dailyTicketRewardsManager;

    private void Awake()
    {
        Instance = this;
#if UNITY_IPHONE
        RequestIosAuthorizationTracking();
#endif
    }

    private void Start()
    {
        InitializeIronSource();
        dailyTicketRewardsManager.Init();
    }

#if UNITY_IPHONE
    private void RequestIosAuthorizationTracking()
    {
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
    }
#endif

    private void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    private void InitializeIronSource()
    {
        if (InDebugMode())
        {
            IronSource.Agent.setMetaData("is_test_suite", "enable");
        }

        IronSource.Agent.shouldTrackNetworkState(true);
        IronSource.Agent.init(PrivateKey.IronSourceAppKey, IronSourceAdUnits.REWARDED_VIDEO);
        SubscribeToIronSourceEvents();
    }

    private void SubscribeToIronSourceEvents()
    {
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnRewardedVideoAdShowFailed;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += OnRewardedVideoAdRewarded;
    }

    private bool InDebugMode()
    {
        return PlayerPrefs.GetInt(PlayerData.DEBUG_MODE, 0) == 1;
    }

    private void OnRewardedVideoAdRewarded(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        UnPauseBackgroundMusic();
        OnAdReward?.Invoke();
    }

    private void OnRewardedVideoAdShowFailed(IronSourceError error, IronSourceAdInfo adInfo)
    {
        PopupTextManager.Instance.ShowOKPopup($"Failed to load AD network: {error}", OnAdFailed);
        UnPauseBackgroundMusic();
    }

    public void ShowAds(Action rewardCallback = null, Action failedCallback = null, string adNote = null)
    {
        OnAdReward = rewardCallback;
        OnAdFailed = failedCallback;

        if (InDebugMode())
        {
            OnAdReward?.Invoke();
            return;
        }

#if !UNITY_EDITOR
        LogFirebaseEvent("Ads", "AdType", adNote);
#endif

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
            PauseBackgroundMusic();
        }
        else
        {
            PopupTextManager.Instance.ShowOKPopup("Reward Video not available. Try again later.", OnAdFailed);
        }
    }

#if !UNITY_EDITOR
    private void LogFirebaseEvent(string eventName, string parameterName, string parameterValue)
    {
        FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
    }
#endif

    private void PauseBackgroundMusic()
    {
        AudioCtrl.Instance.PauseBGM();
        sfxctrl.PauseBGM();
    }

    private void UnPauseBackgroundMusic()
    {
        AudioCtrl.Instance.UnPauseBgm();
        sfxctrl.UnPauseBGM();
    }
}
