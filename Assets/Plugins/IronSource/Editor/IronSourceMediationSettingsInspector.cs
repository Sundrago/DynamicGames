using System.IO;
using UnityEditor;

[CustomEditor(typeof(IronSourceMediationSettings))]
public class IronSourceMediationSettingsInspector : Editor
{
    private static IronSourceMediationSettings ironSourceMediationSettings;

    public static IronSourceMediationSettings IronSourceMediationSettings
    {
        get
        {
            if (ironSourceMediationSettings == null)
            {
                ironSourceMediationSettings =
                    AssetDatabase.LoadAssetAtPath<IronSourceMediationSettings>(IronSourceMediationSettings
                        .IRONSOURCE_SETTINGS_ASSET_PATH);
                if (ironSourceMediationSettings == null)
                {
                    var asset = CreateInstance<IronSourceMediationSettings>();
                    Directory.CreateDirectory(IronSourceConstants.IRONSOURCE_RESOURCES_PATH);
                    AssetDatabase.CreateAsset(asset, IronSourceMediationSettings.IRONSOURCE_SETTINGS_ASSET_PATH);
                    ironSourceMediationSettings = asset;
                }
            }

            return ironSourceMediationSettings;
        }
    }
}