using System;
using System.Collections;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Actions
{
    [Serializable]
    [CreateAssetMenu(fileName = "WaitFor Action", menuName = "Text Animator/Actions/Wait For", order = 1)]
    [TagInfo("waitfor")]
    public sealed class WaitForAction : ActionScriptableBase
    {
        /// <summary>
        ///     Time used in case the action does not have the first parameter
        /// </summary>
        [Tooltip("Time used in case the action does not have the first parameter")]
        public float defaultTime = 1;

        public override IEnumerator DoAction(ActionMarker action, TypewriterCore typewriter, TypingInfo typingInfo)
        {
            var targetTime = defaultTime;
            if (action.parameters.Length > 0)
                FormatUtils.TryGetFloat(action.parameters[0], defaultTime, out targetTime);

            float t = 0;
            while (t <= targetTime)
            {
                t += typewriter.TextAnimator.time.deltaTime;
                yield return null;
            }
        }
    }
}