#if !UNITY_EDITOR
using Firebase.Analytics;
#endif
using System;
using MyUtility;
using UnityEngine;
#if UNITY_IPHONE
using Unity.Advertisement.IosSupport;
#endif

namespace Core.System
{
    public class ADManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] [SerializeField]
        private SfxController sfxController;

        [SerializeField] private DailyTicketRewardsManager dailyTicketRewardsManager;

        public static ADManager Instance { get; private set; }
        private event Action OnAdReward, OnAdFailed;

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
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif

        private void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }

        private void InitializeIronSource()
        {
            if (InDebugMode()) IronSource.Agent.setMetaData("is_test_suite", "enable");

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
            return PlayerData.GetInt(DataKey.debugMode) == 1;
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
            AudioManager.Instance.PauseBGM();
            sfxController.PauseBGM();
        }

        private void UnPauseBackgroundMusic()
        {
            AudioManager.Instance.ResumeBgm();
            sfxController.ResumeBGM();
        }
    }
}