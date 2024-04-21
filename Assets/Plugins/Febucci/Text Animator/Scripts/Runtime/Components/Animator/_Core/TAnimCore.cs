using System;
using System.Collections.Generic;
using System.Text;
using Febucci.UI.Actions;
using Febucci.UI.Core.Parsing;
using Febucci.UI.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Febucci.UI.Core
{
    [DisallowMultipleComponent]
    [HelpURL("https://www.febucci.com/text-animator-unity/docs/how-to-add-effects-to-your-texts/")]
    public abstract class TAnimCore : MonoBehaviour
    {
        public enum DefaultTagsMode
        {
            /// <summary>
            ///     Applies effects only to characters that don't have any.
            /// </summary>
            Fallback = 0,

            /// <summary>
            ///     Applies effects to all the characters, even if they already have other tags via text.
            /// </summary>
            Constant = 1
        }

        /// <summary>
        ///     Contains TextAnimator's current time values.
        /// </summary>
        [HideInInspector] public TimeData time;

        /// <summary>
        ///     Controls how default tags should be applied.\n"Fallback" will apply the effects only to characters that don't have
        ///     any.\n"Constant" will apply the default effects to all the characters, even if they already have other tags via
        ///     text.
        /// </summary>
        [Tooltip(
            "Controls how default tags should be applied.\n\"Fallback\" will apply the effects only to characters that don't have any.\n\"Constant\" will apply the default effects to all the characters, even if they already have other tags via text.")]
        public DefaultTagsMode defaultTagsMode = DefaultTagsMode.Fallback;

        private bool requiresMeshUpdate;

        private void Awake()
        {
            requiresTagRefresh = true;
            TryInitializing();
        }

        #region Callbacks

        protected virtual void OnEnable() // things might have changed when disabled, e.g. autoSize etc.
        {
            requiresMeshUpdate = true;
            AnimateText();
        }

        #endregion

        private void ForceMeshUpdate()
        {
            requiresMeshUpdate = false;
            OnForceMeshUpdate();
        }

        private void TryInitializing()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                if (characters == null) //forces reset in editor
                    initialized = false;
#endif

            if (initialized) return;

            initialized = true;

            TextUtilities.Initialize();
            CharactersCount = 0;
            characters = new CharacterData[0];

            WordsCount = 0;
            words = new WordInfo[0];

            behaviors = new AnimationRegion[0];
            appearances = new AnimationRegion[0];
            disappearances = new AnimationRegion[0];
            Actions = new ActionMarker[0];
            Events = new EventMarker[0];

            if (DatabaseActions) DatabaseActions.ForceBuildRefresh();
            if (DatabaseAppearances) DatabaseAppearances.ForceBuildRefresh();
            if (DatabaseBehaviors) DatabaseBehaviors.ForceBuildRefresh();

            OnInitialized();
        }


        private void UpdateUniformIntensity()
        {
            if (useDynamicScaling)
                for (var i = 0; i < characters.Length; i++)
                    // multiplies by current character size, which could be modified by "size" tags and so
                    // be different than the basic tmp font size value 
                    characters[i].UpdateIntensity(referenceFontSize);
            else
                for (var i = 0; i < characters.Length; i++)
                    characters[i].uniformIntensity = 1;
        }

        /// <summary>
        ///     Schedules that a mesh refresh is required as soon as possible, which will be applied before the next animation loop
        ///     starts.
        /// </summary>
        public void ScheduleMeshRefresh()
        {
            requiresMeshUpdate = true;
        }

        public void ForceDatabaseRefresh()
        {
            if (DatabaseActions) DatabaseActions.ForceBuildRefresh();
            if (DatabaseAppearances) DatabaseAppearances.ForceBuildRefresh();
            if (DatabaseBehaviors) DatabaseBehaviors.ForceBuildRefresh();

            ConvertText(GetOriginalTextFromSource(), ShowTextMode.Refresh);
        }

        /// <summary>
        ///     Enables or disables behavior effects animation *LOCALLY* on this Text Animator component.
        ///     To change this globally, see <see cref="TextAnimatorSettings.SetBehaviorsActive" />
        /// </summary>
        /// <param name="isCategoryEnabled"></param>
        public void SetBehaviorsActive(bool isCategoryEnabled)
        {
            isAnimatingBehaviors = isCategoryEnabled;
        }

        /// <summary>
        ///     Enables or disables appearance effects animation *LOCALLY* on this Text Animator component.
        ///     To change this globally, see <see cref="TextAnimatorSettings.SetAppearancesActive" />
        /// </summary>
        /// <param name="isCategoryEnabled"></param>
        public void SetAppearancesActive(bool isCategoryEnabled)
        {
            isAnimatingAppearances = isCategoryEnabled;
        }

        public void ResetState()
        {
            _text = string.Empty;
            textWithoutTextAnimTags = string.Empty;
            textWithoutAnyTag = string.Empty;
            CharactersCount = 0;
            WordsCount = 0;
            initialized = false;
            TryInitializing();
        }

        #region Types

        private enum ShowTextMode : byte
        {
            Hidden = 0,
            Shown = 1,
            UserTyping = 2,
            Refresh = 3
        }

        #endregion

        #region Variables

        private bool initialized;
        private bool requiresTagRefresh;

        #region Options

        /// <summary>
        ///     If the source text changes, should the typewriter start automatically?
        /// </summary>
        /// <remarks>
        ///     Requires a Typewriter component if true.
        /// </remarks>
        [Tooltip(
            "If the source text changes, should the typewriter start automatically? Requires a Typewriter component if true.\nP.s. Previously, this option was called 'Use Easy Integration'.")]
        public bool typewriterStartsAutomatically;

        private TypewriterCore _typewriterCache;

        /// <summary>
        ///     Linked TAnimPlayer to this component
        /// </summary>
        private TypewriterCore typewriter
        {
            get
            {
                if (_typewriterCache != null)
                    return _typewriterCache;

#if UNITY_2019_2_OR_NEWER
                if (!TryGetComponent(out _typewriterCache))
                    Debug.LogError(
                        $"Typewriter component is null on GameObject {gameObject.name}. Please add a typewriter on the same GameObject or set 'Typewriter Starts Automatically' to false.",
                        gameObject);
#else
                _tAnimPlayer = GetComponent<TAnimPlayerBase>();
                Assert.IsNotNull(_tAnimPlayer, $"Text Animator Player component is null on GameObject {gameObject.name}");
#endif

                return _typewriterCache;
            }
        }


        /// <summary>
        ///     Controls when Text Animator should update its effects. Set it to <see cref="AnimationLoop.Script" /> if you want to
        ///     control the animations from your own loop, invoking the <see cref="Animate(float)" /> method.
        /// </summary>
        [Tooltip(
            "Controls when this TextAnimator component should update its effects. Defaults in the 'Update' Loop.\nSet it to 'Manual' if you want to control the animations from your own loop instead.")]
        public AnimationLoop animationLoop = AnimationLoop.Update;

        /// <summary>
        ///     Chooses which Time Scale to use when automatically animating effects (in other words, when the Update Mode is not
        ///     set to Script). Set it to <see cref="TimeScale.Unscaled" /> if you want to animate effects even when the game is
        ///     paused.
        /// </summary>
        [Tooltip(
            "Chooses which Time Scale to use when animating effects.\nSet it to 'Unscaled' if you want to animate effects even when the game is paused.")]
        public TimeScale timeScale = TimeScale.Scaled;

        #endregion

        #region Text

        /// <summary>
        ///     The original text pasted to Text Animator, with all its tags
        /// </summary>
        [SerializeField] [TextArea(4, 10)] [HideInInspector]
        private string _text = string.Empty;

        public string textFull
        {
            get => _text;
            set
            {
                if (typewriterStartsAutomatically
#if UNITY_EDITOR
                    && Application.isPlaying
#endif
                    && typewriter
                   )
                {
                    SetTypewriterText(value);
                    return;
                }

                SetText(value);
            }
        }

        /// <summary>
        ///     The text without any Text Animator tag
        /// </summary>
        /// <remarks>
        ///     PS. this might still contain other tags from different supported plugins, like "color" from TMPro.
        ///     To get the full stripped text, see <see cref="textWithoutAnyTag" />.
        /// </remarks>
        public string textWithoutTextAnimTags { get; private set; } = string.Empty;

        public string textWithoutAnyTag { get; private set; } = string.Empty;

        private bool hasText => CharactersCount > 0;

        public CharacterData latestCharacterShown { get; private set; }

        /// <summary>
        ///     <c>true</c> if the text is entirely visible, including waiting for appearance effects to finish
        ///     (as they might still hide a character until the very end)
        /// </summary>
        /// <remarks>
        ///     You can use this to check if all the letters have been shown.
        /// </remarks>
        public bool allLettersShown
        {
            get
            {
                if (_maxVisibleCharacters < CharactersCount) return false;
                if (firstVisibleCharacter == _maxVisibleCharacters) return false;

                for (var i = 0; i < CharactersCount; i++)
                    if (!characters[i].isVisible)
                    {
                        if (characters[i].passedTime <= 0)
                            return false;
                    }
                    else
                    {
                        if (characters[i].info.isRendered &&
                            characters[i].passedTime < characters[i].info.appearancesMaxDuration)
                            return false;
                    }

                return true;
            }
        }


        /// <summary>
        ///     <c>true</c> if any letter is still visible in the text
        /// </summary>
        /// <remarks>
        ///     You can use this to check if the disappearance effects are still running.
        /// </remarks>
        public bool anyLetterVisible //TODO test 
        {
            get
            {
                if (characters.Length == 0) return true;

                bool IsCharacterVisible(int index)
                {
                    return characters[index].passedTime > 0;
                }

                //searches for the first character or the last one first, since they're most probably the first ones to be shown (based on orientation)
                if (IsCharacterVisible(0) || IsCharacterVisible(CharactersCount - 1))
                    return true;

                //searches for the other, which might still be running their appearance/disappearance
                for (var i = 1; i < CharactersCount - 1; i++)
                    if (IsCharacterVisible(i))
                        return true;

                return false;
            }
        }


        /// <summary>
        ///     Number of characters in the text
        /// </summary>
        public int CharactersCount { get; private set; }

        private CharacterData[] characters;

        /// <summary>
        ///     The array of characters currently present in the text.
        /// </summary>
        /// <remarks>
        ///     This array might be larger than the actual number of characters, so please cycle for <see cref="CharactersCount" />
        ///     instead.
        /// </remarks>
        public CharacterData[] Characters => characters;

        /// <summary>
        ///     Number of words in the text
        /// </summary>
        public int WordsCount { get; private set; }

        private WordInfo[] words;

        /// <summary>
        ///     The array of words currently present in the text.
        /// </summary>
        /// <remarks>
        ///     This array might be larger than the actual number of words, so please cycle for <see cref="WordsCount" /> instead.
        /// </remarks>
        public WordInfo[] Words => words;

        //---CHARS SIZE/INTENSITY---

        /// <summary>
        ///     True if you want the animations to be uniform/consistent across different font sizes. Default/Suggested to leave
        ///     this as true, and change <see cref="referenceFontSize" />. Otherwise, effects will move more when the text is
        ///     smaller (requires less space on screen).
        /// </summary>
        [Tooltip(
            "True if you want the animations to be uniform/consistent across different font sizes. Default/Suggested to leave this as true, and change the 'Reference Font Size'.\nOtherwise, effects will move more when the text is smaller (requires less space on screen)")]
        public bool useDynamicScaling = true;

        /// <summary>
        ///     Font size that will be used as reference to keep animations consistent/uniform at different scales. Works only if
        ///     <see cref="useDynamicScaling" /> is set to true.
        /// </summary>
        [Tooltip("Font size that will be used as reference to keep animations consistent/uniform at different scales.")]
        public float referenceFontSize = 10;


        //---OTHERS---

        /// <summary>
        ///     True if you want the animator's time to be reset on new text.
        /// </summary>
        [Tooltip("True if you want the animator's time to be reset on new text.")]
        [FormerlySerializedAs("isResettingEffectsOnNewText")]
        public bool isResettingTimeOnNewText = true;

        #endregion

        #region Effects and Databases

        private bool isAnimatingBehaviors = true;
        private bool isAnimatingAppearances = true;

        /// <summary>
        ///     True if you want to use the databases referenced in the <see cref="TextAnimatorSettings" /> asset, otherwise you
        ///     can specify which databases to use in this component.
        /// </summary>
        [Tooltip(
            "Lets you use the databases referenced in the 'TextAnimatorSettings' asset.\nSet to false if you'd like to specify which databases to use in this component.")]
        public bool useDefaultDatabases = true;

        // ----------------
        // -- Databases --
        // ----------------
        [SerializeField] private AnimationsDatabase databaseBehaviors;

        /// <summary>
        ///     Behaviors Database used by this Text Animator. If <see cref="useDefaultDatabases" /> is set to true, this will be
        ///     the default database from the <see cref="TextAnimatorSettings" /> asset.
        /// </summary>
        public AnimationsDatabase DatabaseBehaviors
        {
            get => useDefaultDatabases ? TextAnimatorSettings.Instance.behaviors.defaultDatabase : databaseBehaviors;
            set
            {
                useDefaultDatabases = false;
                databaseBehaviors = value;
                requiresTagRefresh = true;
            }
        }

        [SerializeField] private AnimationsDatabase databaseAppearances;

        /// <summary>
        ///     Appearances Database used by this Text Animator. If <see cref="useDefaultDatabases" /> is set to true, this will be
        ///     the default database from the <see cref="TextAnimatorSettings" /> asset.
        /// </summary>
        public AnimationsDatabase DatabaseAppearances
        {
            get => useDefaultDatabases
                ? TextAnimatorSettings.Instance.appearances.defaultDatabase
                : databaseAppearances;
            set
            {
                useDefaultDatabases = false;
                databaseAppearances = value;
                requiresTagRefresh = true;
            }
        }

        // ----------------
        // -- Effects --
        // ----------------
        private AnimationRegion[] behaviors;

        /// <summary>
        ///     All the behavior effects that are applied to the current text.
        /// </summary>
        public AnimationRegion[] Behaviors
        {
            get => behaviors;
            set => behaviors = value;
        }

        private AnimationRegion[] appearances;

        /// <summary>
        ///     All the appearance effects that are applied to the current text.
        /// </summary>
        public AnimationRegion[] Appearances
        {
            get => appearances;
            set => appearances = value;
        }

        private AnimationRegion[] disappearances;

        /// <summary>
        ///     All the disappearance effects that are applied to the current text.
        /// </summary>
        public AnimationRegion[] Disappearances
        {
            get => disappearances;
            set => disappearances = value;
        }

        #endregion

        #region Actions and Events

        /// <summary>
        ///     All the actions that have been parsed from the current text, and that will be used by a
        ///     <see cref="TypewriterCore" /> component if present.
        /// </summary>
        public ActionMarker[] Actions { get; set; }


        [SerializeField] private ActionDatabase databaseActions;

        /// <summary>
        ///     Actions Database used by this Text Animator. If <see cref="useDefaultDatabases" /> is set to true, this will be the
        ///     default database from the <see cref="TextAnimatorSettings" /> asset.
        /// </summary>
        public ActionDatabase DatabaseActions
        {
            get => useDefaultDatabases ? TextAnimatorSettings.Instance.actions.defaultDatabase : databaseActions;
            set
            {
                databaseActions = value;
                requiresTagRefresh = true;
            }
        }

        /// <summary>
        ///     Events that have been parsed from the current text, and that will be used by a <see cref="TypewriterCore" />
        ///     component if present.
        /// </summary>
        public EventMarker[] Events { get; set; }

        #endregion

        #region Default Tags

        private struct DefaultRegion
        {
            public readonly string[] tagWords;
            public readonly AnimationRegion region;

            public DefaultRegion(string tagID, VisibilityMode visibilityMode, AnimationScriptableBase scriptable,
                string[] tagWords)
            {
                this.tagWords = tagWords;
                region = new AnimationRegion(tagID, visibilityMode, scriptable);
            }
        }

        [SerializeField] private string[] defaultAppearancesTags = { "size" };

        /// <summary>
        ///     Fallback/Constant tags that will be applied to the entire text, if no other tags are found, based on the
        ///     <see cref="defaultTagsMode" /> value.
        /// </summary>
        public string[] DefaultAppearancesTags
        {
            get => defaultAppearancesTags;
            set
            {
                defaultAppearancesTags = value;
                requiresTagRefresh = true;
            }
        }

        [SerializeField] private string[] defaultDisappearancesTags = { "fade" };

        /// <summary>
        ///     Fallback/Constant tags that will be applied to the entire text, if no other tags are found, based on the
        ///     <see cref="defaultTagsMode" /> value.
        /// </summary>
        public string[] DefaultDisappearancesTags
        {
            get => defaultDisappearancesTags;
            set
            {
                defaultDisappearancesTags = value;
                requiresTagRefresh = true;
            }
        }

        [SerializeField] private string[] defaultBehaviorsTags;

        /// <summary>
        ///     Fallback/Constant tags that will be applied to the entire text, if no other tags are found, based on the
        ///     <see cref="defaultTagsMode" /> value.
        /// </summary>
        public string[] DefaultBehaviorsTags
        {
            get => defaultBehaviorsTags;
            set
            {
                defaultBehaviorsTags = value;
                requiresTagRefresh = true;
            }
        }

        #endregion

        #endregion

        #region Abstract / Virtual

        /// <summary>
        ///     Called once when the component is initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        //----------------
        //--Setting Text--
        //----------------
        public abstract string GetOriginalTextFromSource();
        public abstract string GetStrippedTextFromSource();
        public abstract void SetTextToSource(string text);

        //----------------
        //--Checking Changes--
        //----------------
        protected abstract bool HasChangedText(string strippedText);
        protected abstract bool HasChangedRenderingSettings();

        //-----------------
        //--Setting Chars--
        //-----------------
        protected abstract int GetCharactersCount();

        //-----------------
        //---Setting Mesh--
        //-----------------
        protected abstract void OnForceMeshUpdate();
        protected abstract void CopyMeshFromSource(ref CharacterData[] characters);
        protected abstract void PasteMeshToSource(CharacterData[] characters);

        #endregion

        #region Text

        protected virtual TagParserBase[] GetExtraParsers()
        {
            return Array.Empty<TagParserBase>();
        }

        private TextAnimatorSettings settings;

        private void ConvertText(string textToParse, ShowTextMode showTextMode)
        {
            #region Local Methods

            void PopulateCharacters()
            {
                if (characters.Length < CharactersCount)
                    Array.Resize(ref characters, CharactersCount);

                for (var i = 0; i < CharactersCount; i++)
                {
                    //--Resets info--
                    characters[i].ResetInfo(i);

                    //--Assigns effect times--
                    float CalculateRegionMaxDuration(AnimationRegion[] tags)
                    {
                        float maxDuration = 0;
                        float currentDuration;
                        //For each tag
                        foreach (var tag in tags)
                            //for each range
                        foreach (var range in tag.ranges)
                            //If the region contains the character
                            if (i >= range.indexes.x && i < range.indexes.y)
                            {
                                tag.SetupContextFor(this, range.modifiers);

                                currentDuration = tag.animation.GetMaxDuration();

                                //If the region has a duration greater than the current max
                                if (currentDuration > maxDuration)
                                    //Assigns the new max
                                    maxDuration = currentDuration;
                            }

                        return maxDuration;
                    }

                    characters[i].info.disappearancesMaxDuration = CalculateRegionMaxDuration(disappearances);
                    characters[i].info.appearancesMaxDuration = CalculateRegionMaxDuration(appearances);
                }
            }

            void CalculateWords()
            {
                var currentWord = new StringBuilder();
                WordsCount = CharactersCount;

                if (words.Length < WordsCount)
                    Array.Resize(ref words, WordsCount);

                var tempLength = 0;
                var wordIndex = 0;
                var currentFirstIndex = 0;
                for (var i = 0; i < CharactersCount; i++)
                {
                    if (!char.IsWhiteSpace(characters[i].info.character))
                    {
                        characters[i].wordIndex = wordIndex;
                        currentWord.Append(characters[i].info.character);
                        tempLength++;
                        continue;
                    }

                    characters[i].wordIndex = -1;

                    if (tempLength > 0)
                    {
                        words[wordIndex] = new WordInfo(
                            currentFirstIndex,
                            currentFirstIndex + tempLength - 1,
                            currentWord.ToString());
                        currentFirstIndex += tempLength + 1; //removes additional space
                        wordIndex++;
                    }
                    else
                    {
                        currentFirstIndex++; //proceeds to shift white spaces etc.
                    }

                    currentWord.Clear();
                    tempLength = 0;
                }

                //Adds last
                if (tempLength > 0)
                {
                    words[wordIndex] = new WordInfo(
                        currentFirstIndex,
                        currentFirstIndex + tempLength - 1,
                        currentWord.ToString());
                    wordIndex++;
                }

                WordsCount = wordIndex;
            }

            void HideCharacterTime(int charIndex)
            {
                var c = characters[charIndex];
                c.isVisible = false;
                c.passedTime = 0;
                c.Hide();
                characters[charIndex] = c;
            }

            void HideAllCharactersTime()
            {
                for (var i = 0; i < CharactersCount; i++) HideCharacterTime(i);
            }

            void ShowCharacterTimes()
            {
                for (var i = 0; i < CharactersCount; i++)
                {
                    var c = characters[i];
                    c.isVisible = true;
                    c.passedTime = c.info.appearancesMaxDuration;
                    characters[i] = c;
                }
            }

            bool IsCharacterInsideAnyEffect(int charIndex, AnimationRegion[] regions)
            {
                foreach (var region in regions)
                foreach (var range in region.ranges)
                    if (charIndex >= range.indexes.x &&
                        (range.indexes.y == int.MaxValue || charIndex < range.indexes.y))
                        return true;

                return false;
            }

            void AddFallbackEffectsFor<T>(ref AnimationRegion[] currentEffects, VisibilityMode visibilityMode,
                Database<T> database, string[] defaultEffectsTags) where T : AnimationScriptableBase
            {
                if (!database) return;

                if (defaultEffectsTags == null || defaultEffectsTags.Length == 0) return;

                //create list of default regions that should be added
                var defaultRegions = new List<DefaultRegion>();
                string[] tagWords;
                string tagName;
                foreach (var tag in defaultEffectsTags)
                {
                    if (string.IsNullOrEmpty(tag))
                    {
                        if (Application.isPlaying)
                            Debug.LogError($"Empty tag as default effect in database {database.name}. Skipping.",
                                gameObject);
                        continue;
                    }

                    tagWords = tag.Split(' ');
                    tagName = tagWords[0];

                    if (!database.ContainsKey(tagName))
                    {
                        if (Application.isPlaying)
                            Debug.LogError(
                                $"Fallback effect with tag '{tagName}' not found in database {database.name}. Skipping.",
                                gameObject);
                        continue;
                    }

                    defaultRegions.Add(new DefaultRegion(tagName, visibilityMode, database[tagName], tagWords));
                }

                //if there are no current effects, directly adds the default effects
                if (currentEffects.Length == 0 || defaultTagsMode == DefaultTagsMode.Constant)
                    foreach (var element in defaultRegions)
                        element.region.OpenNewRange(0, element.tagWords);
                else
                    //for every character in the text
                    for (var startIndex = 0; startIndex < CharactersCount; startIndex++)
                        //if the character has no effect of this category assigned
                        if (!IsCharacterInsideAnyEffect(startIndex, currentEffects))
                        {
                            //opens new range for default effects
                            foreach (var element in defaultRegions)
                                //add the default effect to the character
                                //TODO performance can be improved by caching modifiers
                                element.region.OpenNewRange(startIndex, element.tagWords);

                            //until there are characters that are not inside this category
                            var endIndex = startIndex + 1;
                            for (; endIndex < CharactersCount; endIndex++)
                                if (IsCharacterInsideAnyEffect(endIndex, currentEffects))
                                    break;

                            //closes new range for default effects
                            foreach (var element in defaultRegions) element.region.TryClosingRange(endIndex);

                            startIndex = endIndex;
                        }

                //adds the default regions to the current effects
                var prevCount = currentEffects.Length;
                Array.Resize(ref currentEffects, currentEffects.Length + defaultRegions.Count);
                for (var i = 0; i < defaultRegions.Count; i++) currentEffects[prevCount + i] = defaultRegions[i].region;
            }

            #endregion

            TryInitializing();

            requiresTagRefresh = false;
            _text = textToParse;

            settings = TextAnimatorSettings.Instance;
            if (!settings)
            {
                CharactersCount = 0;
                Debug.LogError("Text Animator Settings not found. Skipping setting the text to Text Animator.");
                return;
            }

            // Uses default database from settings
            if (useDefaultDatabases)
            {
                databaseBehaviors = settings.behaviors.defaultDatabase;
                databaseAppearances = settings.appearances.defaultDatabase;
                databaseActions = settings.actions.defaultDatabase;
            }


            var ruleBehavior = new AnimationParser<AnimationScriptableBase>(settings.behaviors.openingSymbol, '/',
                settings.behaviors.closingSymbol, VisibilityMode.Persistent, databaseBehaviors);
            var ruleAppearance = new AnimationParser<AnimationScriptableBase>(settings.appearances.openingSymbol, '/',
                settings.appearances.closingSymbol, VisibilityMode.OnVisible, databaseAppearances);
            var ruleDisappearance = new AnimationParser<AnimationScriptableBase>(settings.appearances.openingSymbol,
                '/', '#', settings.appearances.closingSymbol, VisibilityMode.OnHiding, databaseAppearances);
            var ruleActions = new ActionParser(settings.actions.openingSymbol, '/', settings.actions.closingSymbol,
                databaseActions);
            var ruleEvents = new EventParser('<', '/', '>');

            //TODO optimize
            var parsers = new List<TagParserBase>
            {
                ruleBehavior,
                ruleAppearance,
                ruleDisappearance,
                ruleActions,
                ruleEvents
            };

            foreach (var extraParser in GetExtraParsers()) parsers.Add(extraParser);

            //Convert text in tags, mesh etc.
            textWithoutTextAnimTags = TextParser.ParseText(_text, parsers.ToArray());

            //Set converted text to source
            SetTextToSource(textWithoutTextAnimTags);
            textWithoutAnyTag = GetStrippedTextFromSource();
            CharactersCount = GetCharactersCount();

            //Assigns results
            behaviors = ruleBehavior.results;
            appearances = ruleAppearance.results;
            disappearances = ruleDisappearance.results;
            Actions = ruleActions.results;
            Events = ruleEvents.results;

            //Adds fallback effects to characters that have no effect assigned
            AddFallbackEffectsFor(ref behaviors, VisibilityMode.Persistent, databaseBehaviors, defaultBehaviorsTags);
            AddFallbackEffectsFor(ref appearances, VisibilityMode.OnVisible, databaseAppearances,
                defaultAppearancesTags);
            AddFallbackEffectsFor(ref disappearances, VisibilityMode.OnHiding, databaseAppearances,
                defaultDisappearancesTags);

            //Initializes only animations that are being used
            foreach (var behavior in behaviors) behavior.animation.InitializeOnce();
            foreach (var appearance in appearances) appearance.animation.InitializeOnce();
            foreach (var disappearance in disappearances) disappearance.animation.InitializeOnce();

            //Prepares Characters
            PopulateCharacters();
            CopyMeshFromSource(ref characters);
            CalculateWords();

            switch (showTextMode)
            {
                case ShowTextMode.Hidden:
                    HideAllCharactersTime();
                    break;

                case ShowTextMode.Shown:
                    ShowCharacterTimes();
                    break;

                //user is typing, the latest letter has time reset
                case ShowTextMode.UserTyping:
                    ShowCharacterTimes();
                    if (CharactersCount > 1)
                    {
                        HideCharacterTime(CharactersCount - 1);
                        characters[CharactersCount - 1].isVisible = true;
                    }

                    break;

                case ShowTextMode.Refresh:
                    //Does nothing
                    break;
            }

            _maxVisibleCharacters = CharactersCount;

            // Makes sure deltaTime is updated instantly, as user might change the timeScale on the same frame as the
            // text is set (or even at Start/Awake) and typewriters might detect deltaTime of 0 and skip showing the text
            time.UpdateDeltaTime(timeScale == TimeScale.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);

            if (isResettingTimeOnNewText && showTextMode != ShowTextMode.Refresh)
                time.RestartTime();
        }

        /// <summary>
        ///     Sets the text to Text Animator, parsing its rich text tags.
        /// </summary>
        /// <param name="text">Full text that you want to paste, including rich text tags.</param>
        /// <remarks>
        ///     This method shows the text instantly. To control if it should be hidden instead, please see
        ///     <see cref="SetText(string, bool)" />.
        /// </remarks>
        public void SetText(string text)
        {
            ConvertText(text, ShowTextMode.Shown);
        }

        /// <summary>
        ///     Sets the text to Text Animator, parsing its rich text tags.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hideText"></param>
        public void SetText(string text, bool hideText)
        {
            ConvertText(text, hideText ? ShowTextMode.Hidden : ShowTextMode.Shown);
        }

        //TODO optimize, only add new stuff without recalculating text
        /// <summary>
        ///     Adds text to the already existing one, parsing its rich text tags.
        /// </summary>
        /// <param name="appendedText">New text that you want to append</param>
        /// <param name="hideText"></param>
        public void AppendText(string appendedText, bool hideText = false)
        {
            //Prevents appending an empty text
            if (string.IsNullOrEmpty(appendedText))
                return;

            //The user is appending to an empty text
            //so we set it instead
            if (!hasText)
            {
                SetText(appendedText, hideText);
                return;
            }

            var previousResettingTime = isResettingTimeOnNewText;
            isResettingTimeOnNewText = false;

            var currentMax = maxVisibleCharacters;
            var currentFirst = firstVisibleCharacter;
            SetText(textFull + appendedText, hideText);

            //restores visibility
            isResettingTimeOnNewText = previousResettingTime;
            maxVisibleCharacters = currentMax;
            firstVisibleCharacter = currentFirst;
            for (var i = firstVisibleCharacter; i < maxVisibleCharacters; i++)
            {
                characters[i].isVisible = true;
                characters[i].passedTime = characters[i].info.appearancesMaxDuration;
            }
        }

        private void SetTypewriterText(string text)
        {
            //temp fix, opening and closing this TMPro tag (which won't be showed in the text, acting like they aren't there) because otherwise
            //there isn't any way to trigger that the text has changed, if it's actually the same as the previous one.
            if (text.Length <= 0) //forces clearing the mesh during the tempFix, without the <noparse> tags
                typewriter.ShowText("");
            else
                typewriter.ShowText($"<noparse></noparse>{text}");
        }

        //TODO TEST
        /// <summary>
        ///     Sets a character visibility.
        /// </summary>
        /// <param name="index">Character's index. See <see cref="CharactersCount" /> and the <see cref="Characters" /> array.</param>
        /// <param name="isVisible">Controls if the character should be visible</param>
        public void SetVisibilityChar(int index, bool isVisible)
        {
            if (index < 0 || index >= CharactersCount) return;
            characters[index].isVisible = isVisible;
            if (isVisible) latestCharacterShown = characters[index];
        }

        //TODO TEST
        /// <summary>
        ///     Sets a word visibility.
        /// </summary>
        /// <param name="index">Word's index. See <see cref="WordsCount" /> and the <see cref="Words" /> array.</param>
        /// <param name="isVisible">Controls if the word should be visible</param>
        public void SetVisibilityWord(int index, bool isVisible)
        {
            if (index < 0 || index >= WordsCount) return;

            var word = words[index];
            for (var i = Mathf.Max(word.firstCharacterIndex, 0);
                 i <= word.lastCharacterIndex && i < CharactersCount;
                 i++) SetVisibilityChar(i, isVisible);
        }


        //TODO Test
        /// <summary>
        ///     Sets the visibility of the entire text, also allowing to play or skip effects.
        /// </summary>
        /// <param name="isVisible"></param>
        /// <param name="canPlayEffects"></param>
        public void SetVisibilityEntireText(bool isVisible, bool canPlayEffects = true)
        {
            for (var i = 0; i < CharactersCount; i++) SetVisibilityChar(i, isVisible);

            if (!canPlayEffects)
            {
                if (isVisible)
                    for (var i = 0; i < CharactersCount; i++)
                        characters[i].passedTime = characters[i].info.appearancesMaxDuration;
                else
                    for (var i = 0; i < CharactersCount; i++)
                        characters[i].passedTime = 0;
            }
        }

        #endregion

        #region Typing

        /// <summary>
        ///     Handles the very first character allowed to be visible in the text.
        /// </summary>
        /// <remarks>
        ///     Be aware that in order to visible, a character also needs to also have its own "visible" flag set to true. See
        ///     <see cref="SetVisibilityChar" /> and <see cref="CharacterData.isVisible" />
        /// </remarks>
        public int firstVisibleCharacter { get; set; }

        private int _maxVisibleCharacters;

        /// <summary>
        ///     Handles the very last character allowed to be visible in the text.
        /// </summary>
        /// <remarks>
        ///     Be aware that in order to visible, a character also needs to also have its own "visible" flag set to true. See
        ///     <see cref="SetVisibilityChar" /> and <see cref="CharacterData.isVisible" />
        /// </remarks>
        public int maxVisibleCharacters
        {
            get => _maxVisibleCharacters;
            set
            {
                if (_maxVisibleCharacters == value) return;

                _maxVisibleCharacters = value;

                //clamps value
                if (_maxVisibleCharacters < 0)
                    _maxVisibleCharacters = 0;
            }
        }

        #endregion


        #region Animation

        private void Update()
        {
            if (!IsReady()) return;

            //--Easy Integration checks--
            if (HasChangedText(textWithoutTextAnimTags))
            {
                if (typewriterStartsAutomatically && typewriter)
                {
                    SetTypewriterText(GetOriginalTextFromSource());
                    return;
                }

                ConvertText(GetOriginalTextFromSource(), ShowTextMode.UserTyping);
                return;
            }

            //--Animates in Core Loop--
            if (animationLoop == AnimationLoop.Update)
                Animate(timeScale == TimeScale.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        private void LateUpdate()
        {
            if (animationLoop == AnimationLoop.LateUpdate)
                Animate(timeScale == TimeScale.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        protected abstract bool IsReady();

        /// <summary>
        ///     Proceeds the text animation with the given deltaTime value.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <example>
        ///     You could use this if <see cref="animationLoop" /> is set to <see cref="AnimationLoop.Script" /> and you want to
        ///     control when to animate the text.
        /// </example>
        public void Animate(float deltaTime)
        {
            if (!IsReady()) return;

            if (requiresTagRefresh)
                ConvertText(_text, ShowTextMode.Refresh);

            time.UpdateDeltaTime(deltaTime);
            time.IncreaseTime();

            AnimateText();
        }

        #region Processing Regions

        private bool IsCharacterAppearing(int i)
        {
            return i >= firstVisibleCharacter && i < _maxVisibleCharacters && characters[i].isVisible;
        }

        private void ProcessAnimationRegions(AnimationRegion[] regions)
        {
            foreach (var region in regions)
            foreach (var range in region.ranges)
            {
                region.SetupContextFor(this, range.modifiers); //TODO index instead of passing modifier by value

                for (var i = range.indexes.x; i < range.indexes.y && i < CharactersCount; i++)
                {
                    if (characters[i].passedTime <= 0) continue;
                    if (!region.IsVisibilityPolicySatisfied(IsCharacterAppearing(i))) continue;

                    if (region.animation.CanApplyEffectTo(characters[i], this))
                        region.animation.ApplyEffectTo(ref characters[i], this);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Main loop
        /// </summary>
        private void AnimateText()
        {
            //no text, skips
            if (!hasText) return;

            TryInitializing(); //called here as well since this might be called from outside

            //Prepare characters
            for (var i = 0; i < CharactersCount && i < characters.Length; i++)
            {
                //forces hiding character if from source it's not rendered
                if (!characters[i].info.isRendered)
                {
                    characters[i].passedTime = 0;
                    characters[i].Hide();
                    continue;
                }

                characters[i].ResetAnimation();

                //Updates passed time
                if (IsCharacterAppearing(i))
                {
                    characters[i].passedTime += time.deltaTime;
                }
                else
                {
                    if (characters[i].passedTime > characters[i].info.disappearancesMaxDuration)
                        characters[i].passedTime = characters[i].info.disappearancesMaxDuration;
                    else
                        characters[i].passedTime -= time.deltaTime;

                    if (characters[i].passedTime <= 0) // "<=" to force hiding characters when TimeScale = 0 
                    {
                        characters[i].passedTime = 0;
                        characters[i].Hide();
                    }
                }
            }

            UpdateUniformIntensity();

            //Processes animations
            //PS Order is important
            if (isAnimatingBehaviors && settings.behaviors.enabled) ProcessAnimationRegions(behaviors);

            if (isAnimatingAppearances && settings.appearances.enabled)
            {
                ProcessAnimationRegions(appearances);
                ProcessAnimationRegions(disappearances);
            }

            //updates source
            PasteMeshToSource(characters);

            //checks for changes in the setting
            if (requiresMeshUpdate || HasChangedRenderingSettings())
            {
                ForceMeshUpdate();
                CopyMeshFromSource(ref characters);
            }
        }

        #endregion


        #region Obsolete

        // Just for compatibility with older versions

        [Obsolete("Use TextAnimatorSettings.SetAllEffectsActive instead")]
        public static void EnableAllEffects(bool enabled)
        {
            TextAnimatorSettings.SetAllEffectsActive(enabled);
        }

        [Obsolete("Use TextAnimatorSettings.SetAppearancesActive instead")]
        public static void EnableAppearances(bool enabled)
        {
            TextAnimatorSettings.SetAppearancesActive(enabled);
        }

        [Obsolete("Use TextAnimatorSettings.SetBehaviorsActive instead")]
        public static void EnableBehaviors(bool enabled)
        {
            TextAnimatorSettings.SetBehaviorsActive(enabled);
        }


        [Obsolete("Use SetAppearancesActive instead")]
        public void EnableAppearancesLocally(bool value)
        {
            SetAppearancesActive(value);
        }

        [Obsolete("Use SetBehaviorsActive instead")]
        public void EnableBehaviorsLocally(bool value)
        {
            SetBehaviorsActive(value);
        }


        /// <summary>
        ///     Turns all characters visible at the end of the frame (i.e. "a typewriter skip")
        /// </summary>
        /// <param name="skipAppearanceEffects">
        ///     Set this to true if you want all letters to appear instantly (without any
        ///     appearance effect)
        /// </param>
        [Obsolete("Use SetVisibilityEntireText instead")]
        public void ShowAllCharacters(bool skipAppearanceEffects)
        {
            SetVisibilityEntireText(true, skipAppearanceEffects);
        }

        [Obsolete("Use 'Animate' instead.")]
        public void UpdateEffects()
        {
            Animate(timeScale == TimeScale.Unscaled ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        [Obsolete(
            "Events are not tied to TextAnimators anymore, but to their Typewriters. Please invoke 'TriggerRemainingEvents' on the Typewriter component instead.")]
        public void TriggerRemainingEvents()
        {
            if (typewriter) typewriter.TriggerRemainingEvents();
        }

        [Obsolete(
            "Events are not tied to TextAnimators anymore, but to their related typewriters. Please invoke 'TriggerVisibleEvents' on the Typewriter component instead.")]
        public void TriggerVisibleEvents()
        {
            if (typewriter) typewriter.TriggerVisibleEvents();
        }

        [Obsolete("Use 'ScheduleMeshRefresh' instead")]
        public void ForceMeshRefresh()
        {
            ScheduleMeshRefresh();
        }


        [Obsolete(
            "To restart TextAnimator's time, please use 'time.RestartTime()'. To skip appearances effects please set 'SetVisibilityEntireText(true, false)' instead")]
        public void ResetEffectsTime(bool skipAppearances)
        {
            time.RestartTime();

            if (skipAppearances) SetVisibilityEntireText(true, false);
        }

        [Obsolete("Please use 'isResettingTimeOnNewText' instead")]
        public bool isResettingEffectsOnNewText => isResettingTimeOnNewText;

        [Obsolete("Please use 'animationLoop' instead")]
        public AnimationLoop updateMode => animationLoop;

        [Obsolete("Events are now handled/stored by Typewriters instead.")]
        public MessageEvent onEvent => typewriter.onMessage;

        [Obsolete("Please use TextAnimatorSettings.Instance.appearances.enabled instead")]
        public static bool effectsAppearancesEnabled => TextAnimatorSettings.Instance.appearances.enabled;

        [Obsolete("Please use TextAnimatorSettings.Instance.behaviors.enabled instead")]
        public static bool effectsBehaviorsEnabled => TextAnimatorSettings.Instance.behaviors.enabled;

        [Obsolete("Please use 'textFull' instead")]
        public string text => textFull;

        [Obsolete("Please change 'referenceFontSize' instead")]
        public float effectIntensityMultiplier
        {
            get => referenceFontSize;
            set => referenceFontSize = value;
        }

        #endregion
    }
}