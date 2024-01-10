using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ads : MonoBehaviour
{
    public static Ads Instance;

    private void Awake()
    {
        Instance = this;
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
    }

// The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo){
    }

// The user completed to watch the video, and should be rewarded.
// The placement parameter will include the reward data.
// When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo){
    }
// The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo){
    }

    public void ShowAds()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo("YOUR_PLACEMENT_NAME");
        }
        else
        {
            
        }
    }
}
