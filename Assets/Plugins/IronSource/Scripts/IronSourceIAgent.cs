using System.Collections.Generic;

public interface IronSourceIAgent
{
    //******************* Base API *******************//

    /// <summary>
    ///     Allows publishers to set configurations for a waterfall of a given ad type.
    /// </summary>
    /// <param name="waterfallConfiguration">The configuration for the given ad types waterfall. </param>
    /// <param name="adFormat">The AdFormat for which to configure the waterfall.</param>
    void SetWaterfallConfiguration(WaterfallConfiguration waterfallConfiguration, AdFormat adFormat);

    void onApplicationPause(bool pause);

    string getAdvertiserId();

    void validateIntegration();

    void shouldTrackNetworkState(bool track);

    bool setDynamicUserId(string dynamicUserId);

    void setAdaptersDebug(bool enabled);

    void setMetaData(string key, string value);

    void setMetaData(string key, params string[] values);

    int? getConversionValue();

    void setManualLoadRewardedVideo(bool isOn);

    void setNetworkData(string networkKey, string networkData);

    void SetPauseGame(bool pause);

    //******************* SDK Init *******************//

    void setUserId(string userId);

    void init(string appKey);

    void init(string appKey, params string[] adUnits);

    void initISDemandOnly(string appKey, params string[] adUnits);

    //******************* RewardedVideo API *******************//

    void loadRewardedVideo();

    void showRewardedVideo();

    void showRewardedVideo(string placementName);

    bool isRewardedVideoAvailable();

    bool isRewardedVideoPlacementCapped(string placementName);

    IronSourcePlacement getPlacementInfo(string name);

    void setRewardedVideoServerParams(Dictionary<string, string> parameters);

    void clearRewardedVideoServerParams();

    //******************* RewardedVideo DemandOnly API *******************//

    void showISDemandOnlyRewardedVideo(string instanceId);

    void loadISDemandOnlyRewardedVideo(string instanceId);

    bool isISDemandOnlyRewardedVideoAvailable(string instanceId);

    //******************* Interstitial API *******************//

    void loadInterstitial();

    void showInterstitial();

    void showInterstitial(string placementName);

    bool isInterstitialReady();

    bool isInterstitialPlacementCapped(string placementName);

    //******************* Interstitial DemandOnly API *******************//

    void loadISDemandOnlyInterstitial(string instanceId);

    void showISDemandOnlyInterstitial(string instanceId);

    bool isISDemandOnlyInterstitialReady(string instanceId);

    //******************* Offerwall API *******************//

    void showOfferwall();

    void showOfferwall(string placementName);

    bool isOfferwallAvailable();

    void getOfferwallCredits();

    //******************* Banner API *******************//

    void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position);

    void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position, string placementName);

    void destroyBanner();

    void displayBanner();

    void hideBanner();

    bool isBannerPlacementCapped(string placementName);

    void setSegment(IronSourceSegment segment);

    void setConsent(bool consent);

    //******************* ConsentView API *******************//

    void loadConsentViewWithType(string consentViewType);

    void showConsentViewWithType(string consentViewType);

    //******************* ILRD API *******************//

    void setAdRevenueData(string dataSource, Dictionary<string, string> impressionData);

    //******************* TestSuite API *******************//

    void launchTestSuite();
}

public static class dataSource
{
    public static string MOPUB => "MoPub";
}


public static class IronSourceAdUnits
{
    public static string REWARDED_VIDEO => "rewardedvideo";

    public static string INTERSTITIAL => "interstitial";

    public static string OFFERWALL => "offerwall";

    public static string BANNER => "banner";
}

public class IronSourceBannerSize
{
    public static IronSourceBannerSize BANNER = new("BANNER");
    public static IronSourceBannerSize LARGE = new("LARGE");
    public static IronSourceBannerSize RECTANGLE = new("RECTANGLE");
    public static IronSourceBannerSize SMART = new("SMART");
    private bool isAdaptive;

    private IronSourceBannerSize()
    {
    }

    public IronSourceBannerSize(int width, int height)
    {
        Width = width;
        Height = height;
        Description = "CUSTOM";
    }

    public IronSourceBannerSize(string description)
    {
        Description = description;
        Width = 0;
        Height = 0;
    }

    public string Description { get; }

    public int Width { get; }

    public int Height { get; }

    public void SetAdaptive(bool adaptive)
    {
        isAdaptive = adaptive;
    }

    public bool IsAdaptiveEnabled()
    {
        return isAdaptive;
    }
}

public enum IronSourceBannerPosition
{
    TOP = 1,
    BOTTOM = 2
}