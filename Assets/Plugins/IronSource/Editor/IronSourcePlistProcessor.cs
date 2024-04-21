#if UNITY_IOS || UNITY_IPHONE

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

/// <summary>
///     PostProcessor script to automatically fill all required dependencies
/// </summary>
public class IronSourcePlistProcessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (File.Exists(IronSourceMediationSettings.IRONSOURCE_SETTINGS_ASSET_PATH))
            if (buildTarget == BuildTarget.iOS)
            {
                /*
                 * PBXProject
                 */
                var plistPath = Path.Combine(buildPath, "Info.plist");
                var project = new PBXProject();
                var projectPath = PBXProject.GetPBXProjectPath(buildPath);
                project.ReadFromFile(projectPath);
                var plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
                if (plist != null)
                {
                    // Get root
                    var rootDict = plist.root;

                    // Check if SKAdNetworkItems already exists
                    PlistElementArray SKAdNetworkItems = null;
                    if (rootDict.values.ContainsKey("SKAdNetworkItems"))
                        try
                        {
                            SKAdNetworkItems = rootDict.values["SKAdNetworkItems"] as PlistElementArray;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(string.Format("Could not obtain SKAdNetworkItems PlistElementArray: {0}",
                                e.Message));
                        }

                    //Add IronSource's SKAdNetwork ID
                    if (IronSourceMediationSettingsInspector.IronSourceMediationSettings.AddIronsourceSkadnetworkID)
                    {
                        // If not exists, create it
                        if (SKAdNetworkItems == null) SKAdNetworkItems = rootDict.CreateArray("SKAdNetworkItems");

                        var plistContent = File.ReadAllText(plistPath);
                        if (!plistContent.Contains(IronSourceConstants.IRONSOURCE_SKAN_ID_KEY))
                        {
                            var SKAdNetworkIdentifierDict = SKAdNetworkItems.AddDict();
                            SKAdNetworkIdentifierDict.SetString("SKAdNetworkIdentifier",
                                IronSourceConstants.IRONSOURCE_SKAN_ID_KEY);
                        }
                    }


                    File.WriteAllText(plistPath, plist.WriteToString());
                }
            }

        if (File.Exists(IronSourceMediatedNetworkSettings.MEDIATION_SETTINGS_ASSET_PATH))
            if (buildTarget == BuildTarget.iOS)
            {
                /*
                 * PBXProject
                 */
                var plistPath = Path.Combine(buildPath, "Info.plist");
                var project = new PBXProject();
                var projectPath = PBXProject.GetPBXProjectPath(buildPath);
                project.ReadFromFile(projectPath);
                var plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));
                if (plist != null)
                {
                    // Get root
                    var rootDict = plist.root;


                    //Adding AdMob App ID to Plist
                    if (IronSourceMediatedNetworkSettingsInspector.IronSourceMediatedNetworkSettings.EnableAdmob)
                    {
                        var appId = IronSourceMediatedNetworkSettingsInspector.IronSourceMediatedNetworkSettings
                            .AdmobIOSAppId;
                        if (appId.Length == 0)
                            StopBuildWithMessage(
                                "iOS AdMob app ID is empty. Please enter your app ID to run ads properly");
                        else if (!Regex.IsMatch(appId, "^[a-zA-Z0-9-~]*$"))
                            StopBuildWithMessage(
                                "iOS AdMob app ID is not valid. Please enter a valid app ID to run ads properly");
                        else
                            plist.root.SetString("GADApplicationIdentifier", appId);
                    }

                    File.WriteAllText(plistPath, plist.WriteToString());
                }
            }
    }

    private static void StopBuildWithMessage(string message)
    {
        var prefix = "[IronSourceApplicationSettings] ";

        EditorUtility.DisplayDialog(
            "IronSource Developer Settings", "Error: " + message, "", "");

#if UNITY_2017_1_OR_NEWER
        throw new BuildPlayerWindow.BuildMethodException(prefix + message);
#else
                    throw new OperationCanceledException(prefix + message);
#endif
    }
}
#endif