using System;

public interface IUnityRewardedVideo
{
    //Rewarded Video mediation callbacks
    event Action<IronSourceError> OnRewardedVideoAdShowFailed;

    event Action OnRewardedVideoAdOpened;

    event Action OnRewardedVideoAdClosed;

    event Action OnRewardedVideoAdStarted;

    event Action OnRewardedVideoAdEnded;

    event Action<IronSourcePlacement> OnRewardedVideoAdRewarded;

    event Action<IronSourcePlacement> OnRewardedVideoAdClicked;

    event Action<bool> OnRewardedVideoAvailabilityChanged;

    //Rewarded Video Demand Only callbacks

    event Action<string> OnRewardedVideoAdOpenedDemandOnlyEvent;

    event Action<string> OnRewardedVideoAdClosedDemandOnlyEvent;

    event Action<string> OnRewardedVideoAdLoadedDemandOnlyEvent;

    event Action<string> OnRewardedVideoAdRewardedDemandOnlyEvent;

    event Action<string, IronSourceError> OnRewardedVideoAdShowFailedDemandOnlyEvent;

    event Action<string> OnRewardedVideoAdClickedDemandOnlyEvent;

    event Action<string, IronSourceError> OnRewardedVideoAdLoadFailedDemandOnlyEvent;
}