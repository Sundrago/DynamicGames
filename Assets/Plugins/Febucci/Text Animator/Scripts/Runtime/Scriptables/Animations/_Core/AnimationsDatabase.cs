using System;
using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    /// <summary>
    ///     Contains animations that will be recognized and used by Text Animator
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Animations Database", menuName = "Text Animator/Animations/Create Animations Database",
        order = 100)]
    public class AnimationsDatabase : Database<AnimationScriptableBase>
    {
    }
}