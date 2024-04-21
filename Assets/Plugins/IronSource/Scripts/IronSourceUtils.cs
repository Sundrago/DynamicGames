using System;
using System.Collections;
using System.Collections.Generic;
using IronSourceJSON;

public class IronSourceUtils
{
    private const string ERROR_CODE = "error_code";
    private const string ERROR_DESCRIPTION = "error_description";
    private const string INSTANCE_ID_KEY = "instanceId";
    private const string PLACEMENT_KEY = "placement";

    public static IronSourceError getErrorFromErrorObject(object descriptionObject)
    {
        Dictionary<string, object> error = null;
        if (descriptionObject is IDictionary)
            error = descriptionObject as Dictionary<string, object>;
        else if (descriptionObject is string && !string.IsNullOrEmpty(descriptionObject.ToString()))
            error = Json.Deserialize(descriptionObject.ToString()) as Dictionary<string, object>;

        var sse = new IronSourceError(-1, "");
        if (error != null && error.Count > 0)
        {
            var eCode = Convert.ToInt32(error[ERROR_CODE].ToString());
            var eDescription = error[ERROR_DESCRIPTION].ToString();
            sse = new IronSourceError(eCode, eDescription);
        }

        return sse;
    }

    public static IronSourcePlacement getPlacementFromObject(object placementObject)
    {
        Dictionary<string, object> placementJSON = null;
        if (placementObject is IDictionary)
            placementJSON = placementObject as Dictionary<string, object>;
        else if (placementObject is string)
            placementJSON = Json.Deserialize(placementObject.ToString()) as Dictionary<string, object>;

        IronSourcePlacement ssp = null;
        if (placementJSON != null && placementJSON.Count > 0)
        {
            var rewardAmount = Convert.ToInt32(placementJSON["placement_reward_amount"].ToString());
            var rewardName = placementJSON["placement_reward_name"].ToString();
            var placementName = placementJSON["placement_name"].ToString();

            ssp = new IronSourcePlacement(placementName, rewardName, rewardAmount);
        }

        return ssp;
    }
}