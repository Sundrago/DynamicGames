using System;
using Febucci.UI.Actions;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI
{
    /// <summary>
    ///     Contains global settings for Text Animator, like effects enabled status and default databases.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Text Animator Settings", menuName = "Text Animator/Settings", order = 100)]
    public sealed class TextAnimatorSettings : ScriptableObject
    {
        public const string expectedName = "TextAnimatorSettings";
        private static TextAnimatorSettings instance;

        [Header("Default info")] public Category<AnimationsDatabase> behaviors = new('<', '>');

        public Category<AnimationsDatabase> appearances = new('{', '}');
        public Category<ActionDatabase> actions = new('<', '>');

        /// <summary>
        ///     The current instance of the settings. If it's null, it will be loaded from the resources.
        ///     (Make sure to have one "TextAnimatorSettings" file in the Resources folder.)
        /// </summary>
        public static TextAnimatorSettings Instance
        {
            get
            {
                if (instance) return instance;

                LoadSettings();
                return instance;
            }
        }

        /// <summary>
        ///     Manually loads the settings ScriptableObject in case it wasn't loaded yet.
        /// </summary>
        public static void LoadSettings()
        {
            if (instance) return;
            instance = Resources.Load<TextAnimatorSettings>(expectedName);
        }

        /// <summary>
        ///     Manually unloads the settings ScriptableObject instance.
        /// </summary>
        public static void UnloadSettings()
        {
            if (!instance) return;

            Resources.UnloadAsset(instance);
            instance = null;
        }

        /// <summary>
        ///     Sets all the effects (both appearances/disappearances and behaviors) status.
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetAllEffectsActive(bool enabled)
        {
            SetAppearancesActive(enabled);
            SetBehaviorsActive(enabled);
        }

        /// <summary>
        ///     Sets all appearances effects status.
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetAppearancesActive(bool enabled)
        {
            if (Instance) Instance.appearances.enabled = enabled;
        }

        /// <summary>
        ///     Sets all behaviors effects status.
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetBehaviorsActive(bool enabled)
        {
            if (Instance) Instance.behaviors.enabled = enabled;
        }

        [Serializable]
        public struct Category<T> where T : ScriptableObject
        {
            public T defaultDatabase;

            public bool enabled;
            public char openingSymbol;
            public char closingSymbol;

            public Category(char openingSymbol, char closingSymbol)
            {
                defaultDatabase = null;
                enabled = true;
                this.openingSymbol = openingSymbol;
                this.closingSymbol = closingSymbol;
            }
        }
    }
}