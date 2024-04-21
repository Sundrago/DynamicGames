using System.Text;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(TAnimCore), true)]
    internal class TAnimCoreDrawer : Editor
    {
        private static readonly string[] excludedProperties =
        {
            "m_Script",
            "_text",
            "databaseBehaviors",
            "databaseAppearances",
            "defaultAppearancesTags",
            "defaultBehaviorsTags",
            "defaultDisappearancesTags",
            "typewriterStartsAutomatically",
            nameof(TAnimCore.referenceFontSize),
            nameof(TAnimCore.useDynamicScaling),
            nameof(TAnimCore.defaultTagsMode),
            nameof(TAnimCore.timeScale),
            nameof(TAnimCore.animationLoop),
            nameof(TAnimCore.isResettingTimeOnNewText),
            "databaseActions",
            "useDefaultDatabases"
        };

        private SerializedProperty animationLoop;
        private DatabaseSharedDrawer databaseActionsDrawer;
        private SerializedProperty databaseActionsField;
        private DatabaseSharedDrawer databaseAppearancesDrawer;
        private SerializedProperty databaseAppearancesField;
        private DatabaseSharedDrawer databaseBehaviorsDrawer;

        private SerializedProperty databaseBehaviorsField;
        private SerializedProperty defaultAppearancesTags;
        private SerializedProperty defaultBehaviorsTags;
        private SerializedProperty defaultDisappearancesTags;


        private SerializedProperty defaultTagsMode;
        private SerializedProperty isResettingTimeOnNewText;
        private SerializedProperty m_Script;

        private SerializedProperty referenceFontSize;

        private TAnimCore script;

        private SerializedProperty timeScale;

        private SerializedProperty typewriterStartsAutomatically;

        private SerializedProperty useDefaultDatabases;
        private SerializedProperty useDynamicScaling;

        private void OnEnable()
        {
            script = (TAnimCore)target;

            m_Script = serializedObject.FindProperty("m_Script");

            typewriterStartsAutomatically = serializedObject.FindProperty("typewriterStartsAutomatically");

            useDefaultDatabases = serializedObject.FindProperty("useDefaultDatabases");
            databaseBehaviorsField = serializedObject.FindProperty("databaseBehaviors");
            databaseAppearancesField = serializedObject.FindProperty("databaseAppearances");
            databaseActionsField = serializedObject.FindProperty("databaseActions");

            databaseBehaviorsDrawer = new DatabaseSharedDrawer();
            databaseAppearancesDrawer = new DatabaseSharedDrawer();
            databaseActionsDrawer = new DatabaseSharedDrawer();

            defaultTagsMode = serializedObject.FindProperty(nameof(TAnimCore.defaultTagsMode));
            defaultAppearancesTags = serializedObject.FindProperty("defaultAppearancesTags");
            defaultBehaviorsTags = serializedObject.FindProperty("defaultBehaviorsTags");
            defaultDisappearancesTags = serializedObject.FindProperty("defaultDisappearancesTags");

            referenceFontSize = serializedObject.FindProperty(nameof(TAnimCore.referenceFontSize));
            useDynamicScaling = serializedObject.FindProperty(nameof(TAnimCore.useDynamicScaling));
            timeScale = serializedObject.FindProperty(nameof(TAnimCore.timeScale));
            animationLoop = serializedObject.FindProperty(nameof(TAnimCore.animationLoop));
            isResettingTimeOnNewText = serializedObject.FindProperty(nameof(TAnimCore.isResettingTimeOnNewText));

            gui_visibleCharacters = new GUIContent("Visible Characters", null,
                $"Range of visible characters in the text.\nTo modify this via script, set \"{nameof(TAnimCore.firstVisibleCharacter)}\" and \"{nameof(TAnimCore.maxVisibleCharacters)}\"");

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            RegisterUndoRedraw();
            UnregisterPlayback();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            UnregisterPlayback();
            UnregisterUndoRedraw();
        }

        private void OnDestroy()
        {
            UnregisterPlayback();
            UnregisterUndoRedraw();
        }

        private void OnPlayModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
                UnregisterPlayback();
        }

        #region Main Settings

        private void DrawMainSettings()
        {
            EditorGUILayout.LabelField("Main Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(typewriterStartsAutomatically);
            if (typewriterStartsAutomatically.boolValue)
                EditorGUILayout.LabelField("(Remember to add a Typewriter component!)",
                    EditorStyles.wordWrappedMiniLabel);

            EditorGUILayout.PropertyField(useDynamicScaling);
            if (useDynamicScaling.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(referenceFontSize);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(animationLoop);
            if (animationLoop.intValue != (int)AnimationLoop.Script)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(timeScale);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.PropertyField(isResettingTimeOnNewText);

            EditorGUI.indentLevel--;
        }

        #endregion

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;
            //--- Draws Text Animator ---

            DrawPlayback();
            EditorGUILayout.Space();

            DrawMainSettings();
            EditorGUILayout.Space();

            DrawDefaultTags();
            EditorGUILayout.Space();

            DrawDatabases();
            EditorGUILayout.Space();

            //--- Draws the rest ---
            //(in case of custom inspector from child classes etc.)
            DrawPropertiesExcluding(serializedObject, excludedProperties);

            if (serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        #region Undo Redo

        private bool undoRedoRegistered;

        private void RegisterUndoRedraw()
        {
            if (undoRedoRegistered)
                return;

            undoRedoRegistered = true;
            Undo.undoRedoPerformed += UndoRedraw;
        }

        private void UndoRedraw()
        {
            OnDisable();
            OnEnable();
            Repaint();
            runInEditMode = false;
        }

        private void UnregisterUndoRedraw()
        {
            if (!undoRedoRegistered)
                return;

            undoRedoRegistered = false;
            Undo.undoRedoPerformed -= UndoRedraw;
        }

        #endregion

        #region Playback

        private string textBeforePreview;
        private string textDuringPreview;

        private bool runInEditMode;
        private bool isPlaying => Application.isPlaying || runInEditMode;

        private GUIContent gui_visibleCharacters;
        private Vector2 playbackCharsScrollView;
        private bool extraPlaybackControls;

        private void DrawPlayback()
        {
            void HookPlaybackEvent()
            {
                script.time.RestartTime();

                if (runInEditMode) RegisterPlayback();
                else UnregisterPlayback();
            }

            //--- Playback Toolbar ---
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Application.isPlaying ? "Playback info" : "Preview in Edit Mode",
                GUILayout.Width(120));
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button(runInEditMode ? TexturesLoader.StopIcon : TexturesLoader.PlayIcon, EditorStyles.label,
                    GUILayout.Width(20), GUILayout.Height(20)))
            {
                runInEditMode = !runInEditMode;
                HookPlaybackEvent();
            }

            GUI.enabled = runInEditMode && !Application.isPlaying;
            if (GUILayout.Button(TexturesLoader.SaveIcon, EditorStyles.label, GUILayout.Width(20),
                    GUILayout.Height(20)))
            {
                textBeforePreview = textDuringPreview;
                runInEditMode = false;
                HookPlaybackEvent();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            //---Visible characters---
            if (isPlaying)
            {
                //---Text---
                if (!Application.isPlaying)
                {
                    EditorGUI.BeginChangeCheck();

                    textDuringPreview = EditorGUILayout.TextArea(textDuringPreview, GUILayout.MinHeight(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        script.SetText(textDuringPreview);
                        EditorUtility.SetDirty(script);
                    }
                }

                //---Visible characters---
                var charCount = script.CharactersCount;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //First visible character
                EditorGUI.BeginChangeCheck();
                float minValue = script.firstVisibleCharacter;
                float maxValue = script.maxVisibleCharacters;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.MinMaxSlider(gui_visibleCharacters, ref minValue, ref maxValue, 0, charCount);
                if (EditorGUI.EndChangeCheck())
                {
                    script.firstVisibleCharacter = Mathf.RoundToInt(minValue);
                    script.maxVisibleCharacters = Mathf.RoundToInt(maxValue);
                }

                EditorGUILayout.Space();
                var intMinValue = script.firstVisibleCharacter;
                var intMaxValue = script.maxVisibleCharacters;
                EditorGUI.BeginChangeCheck();
                intMinValue = EditorGUILayout.DelayedIntField(GUIContent.none, intMinValue, GUILayout.Width(30));
                EditorGUILayout.LabelField("/", GUILayout.Width(10));
                intMaxValue = EditorGUILayout.DelayedIntField(GUIContent.none, intMaxValue, GUILayout.Width(30));
                if (EditorGUI.EndChangeCheck())
                {
                    script.firstVisibleCharacter = Mathf.Clamp(intMinValue, 0, charCount);
                    script.maxVisibleCharacters = Mathf.Clamp(intMaxValue, 0, charCount);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Time passed:");
                if (GUILayout.Button(TexturesLoader.RestartIcon, EditorStyles.label, GUILayout.Width(20),
                        GUILayout.Height(20))) script.time.RestartTime();
                EditorGUILayout.LabelField(script.time.timeSinceStart.ToString("F2"), EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();

                GUI.color = extraPlaybackControls ? Color.gray : Color.white;
                if (GUILayout.Button("Extra Visibility Controls", EditorStyles.helpBox))
                    extraPlaybackControls = !extraPlaybackControls;
                GUI.color = Color.white;

                if (extraPlaybackControls)
                {
                    const float boxSize = 12;

                    if (script.WordsCount > 20)
                        EditorGUILayout.HelpBox("Displaying only the first 20 words to optimize performance",
                            MessageType.None);

                    playbackCharsScrollView = EditorGUILayout.BeginScrollView(playbackCharsScrollView);

                    EditorGUILayout.BeginHorizontal();
                    for (var w = 0; w < script.WordsCount && w < 20; w++) //max X words for performance
                    {
                        EditorGUILayout.BeginVertical();
                        if (GUILayout.Button(script.Words[w].text, EditorStyles.miniButton))
                            script.SetVisibilityWord(w,
                                !script.Characters[script.Words[w].firstCharacterIndex].isVisible);

                        EditorGUILayout.BeginHorizontal();
                        for (var i = script.Words[w].firstCharacterIndex; i <= script.Words[w].lastCharacterIndex; i++)
                            script.Characters[i].isVisible = EditorGUILayout.Toggle(script.Characters[i].isVisible,
                                GUILayout.Width(boxSize));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private bool registeredPlayback;

        private void RegisterPlayback()
        {
            if (registeredPlayback) return;
            registeredPlayback = true;

            if (script)
            {
                textBeforePreview = script.GetOriginalTextFromSource();
                textDuringPreview = textBeforePreview;
                if (textDuringPreview.StartsWith("<noparse></noparse>"))
                    textDuringPreview = textDuringPreview.Remove(0, 19);

                //resets text and databases regardless
                script.ForceDatabaseRefresh();
                script.SetText(textDuringPreview);
            }

            lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += UpdatePlayback;
        }

        private void UnregisterPlayback()
        {
            if (!registeredPlayback) return;
            registeredPlayback = false;

            if (script)
            {
                // unfocus text area field to prevent having the same text when it appears again
                if (textDuringPreview != string.Empty)
                    GUIUtility.keyboardControl = -1;

                script.SetTextToSource(textBeforePreview);
                textBeforePreview = string.Empty;
                textDuringPreview = string.Empty;
            }

            EditorApplication.update -= UpdatePlayback;
        }

        private double lastTime;

        private void UpdatePlayback()
        {
            script.Animate((float)(EditorApplication.timeSinceStartup - lastTime));
            lastTime = EditorApplication.timeSinceStartup;
            EditorApplication.QueuePlayerLoopUpdate();
            Repaint();
        }

        #endregion

        #region Default Tags

        private bool drawDefaultBehaviorTags;
        private bool drawDefaultAppearancesTags;
        private bool drawDefaultDisappearancesTags;

        private void ForceDatabaseRefresh()
        {
            serializedObject.ApplyModifiedProperties();
            script.ForceDatabaseRefresh();
        }

        private void DrawDefaultTags()
        {
            const string helpConstantTags = "How many of these effects will be applied to the entire text";
            const string helpFallbackTags =
                "How many of these effects will be applied to a letter, in case there aren't others of the same category.";

            void DrawDefaultArray<T>(ref bool enabledOption, string name, SerializedProperty array,
                Database<T> database) where T : ScriptableObject, ITagProvider
            {
                var size = array.arraySize;
                EditorGUILayout.BeginHorizontal();
                enabledOption = EditorGUILayout.Foldout(enabledOption, name + $" [{size} enabled]", true);

                var areAllTagsValid = true;

                bool IsTagValid(string tag)
                {
                    // hackyly returns valid on an empty tag, to allow user to add new tags without error
                    return string.IsNullOrEmpty(tag) ||
                           database.ContainsKey(tag.Split(' ')[0]); //splits in case of attributes
                }

                // Displays icon if any of the tags is not found in the database
                if (database)
                {
                    for (var i = 0; i < array.arraySize && areAllTagsValid; i++)
                        if (!IsTagValid(array.GetArrayElementAtIndex(i).stringValue))
                            areAllTagsValid = false;

                    if (!areAllTagsValid)
                        GUILayout.Box(TexturesLoader.WarningIcon, EditorStyles.label, GUILayout.Width(20),
                            GUILayout.Height(20));
                }

                EditorGUILayout.EndHorizontal();
                if (enabledOption)
                {
                    EditorGUI.indentLevel++; //--begin of foldout

                    if (!areAllTagsValid)
                    {
                        var sb = new StringBuilder();
                        foreach (var effect in database.Data)
                        {
                            if (!effect) continue;
                            if (string.IsNullOrEmpty(effect.TagID)) continue;
                            sb.Append(effect.TagID);
                            sb.Append(" ");
                        }

                        EditorGUILayout.HelpBox(
                            $"The tags with the warning icons will not be recognized by Text Animator. Accepted tags: {sb}",
                            MessageType.Warning);
                    }

                    GUI.enabled = false;
                    EditorGUILayout.LabelField(
                        defaultTagsMode.intValue == (int)TAnimCore.DefaultTagsMode.Fallback
                            ? helpFallbackTags
                            : helpConstantTags, EditorStyles.wordWrappedMiniLabel);
                    GUI.enabled = true;

                    //Edits array size
                    EditorGUI.BeginChangeCheck();
                    size = EditorGUILayout.IntField("Effects Count", size);
                    if (EditorGUI.EndChangeCheck())
                    {
                        var increasing = size > array.arraySize;
                        size = Mathf.Clamp(size, 0, size + 1);
                        array.arraySize = size;

                        if (increasing && size > 1) //new element is empty
                            array.GetArrayElementAtIndex(size - 1).stringValue = string.Empty;

                        ForceDatabaseRefresh();
                    }

                    var scrollPos = Vector2.zero;
                    if (size > 0)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.LabelField(
                            "Choose one effect per array element, e.g. 'wave'.\nModifiers are also accepted, e.g. 'wave a=2'",
                            EditorStyles.wordWrappedMiniLabel);
                        GUI.enabled = true;
                        EditorGUI.indentLevel++; //--begin of array

                        var temp = new GUIContent();
                        string tempString;
                        for (var i = 0; i < array.arraySize; i++)
                        {
                            var element = array.GetArrayElementAtIndex(i);
                            temp.text = "Effect #" + (i + 1);
                            tempString = element.stringValue;
                            // Displays a nice set of button the user can choose recognized tags from

                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.BeginChangeCheck();
                            tempString = EditorGUILayout.DelayedTextField(temp, tempString);
                            if (EditorGUI.EndChangeCheck())
                            {
                                element.stringValue = tempString;
                                ForceDatabaseRefresh();
                            }

                            if (database && !IsTagValid(tempString))
                                GUILayout.Box(TexturesLoader.WarningIcon, EditorStyles.label, GUILayout.Width(20),
                                    GUILayout.Height(20));

                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUI.indentLevel--; //--end of array
                    }

                    EditorGUI.indentLevel--; //--end of foldout
                }
            }

            EditorGUILayout.LabelField("Default Tags", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(defaultTagsMode, true);
            if (EditorGUI.EndChangeCheck())
                ForceDatabaseRefresh();

            if (useDefaultDatabases.boolValue)
            {
                var settings = TextAnimatorSettings.Instance;
                if (settings)
                {
                    DrawDefaultArray(ref drawDefaultAppearancesTags, "Appearances", defaultAppearancesTags,
                        settings.appearances.defaultDatabase);
                    DrawDefaultArray(ref drawDefaultBehaviorTags, "Behaviors", defaultBehaviorsTags,
                        settings.behaviors.defaultDatabase);
                    DrawDefaultArray(ref drawDefaultDisappearancesTags, "Disappearances", defaultDisappearancesTags,
                        settings.appearances.defaultDatabase);
                }
            }
            else
            {
                DrawDefaultArray(ref drawDefaultAppearancesTags, "Appearances", defaultAppearancesTags,
                    script.DatabaseAppearances);
                DrawDefaultArray(ref drawDefaultBehaviorTags, "Behaviors", defaultBehaviorsTags,
                    script.DatabaseBehaviors);
                DrawDefaultArray(ref drawDefaultDisappearancesTags, "Disappearances", defaultDisappearancesTags,
                    script.DatabaseAppearances);
            }

            EditorGUI.indentLevel--;
        }

        #endregion

        #region Databases

        private bool editBehaviors;
        private bool editAppearances;
        private bool editActions;
        private SerializedObject settingsObject;

        private void DrawDatabases()
        {
            EditorGUILayout.LabelField("Edit Effects & Actions", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useDefaultDatabases);
            GUI.enabled = false;
            EditorGUILayout.LabelField(
                useDefaultDatabases.boolValue
                    ? "Editing databases for every component that uses default settings."
                    : "Editing databases assigned only to this component.", EditorStyles.wordWrappedMiniLabel);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            void DrawDatabaseField(ref bool foldoutToggle, string foldoutName, SerializedProperty field,
                DatabaseSharedDrawer drawer)
            {
                foldoutToggle = EditorGUILayout.Foldout(foldoutToggle, foldoutName, true);

                if (foldoutToggle)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(field);
                    if (EditorGUI.EndChangeCheck())
                        ForceDatabaseRefresh();
                    drawer.OnInspectorGUI(field);
                    EditorGUI.indentLevel--;
                }
            }

            if (useDefaultDatabases.boolValue)
            {
                var settings = TextAnimatorSettings.Instance;
                if (!settings)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox(
                        "Text Animator Settings not found. Please re-run the setup wizard or click the following button.",
                        MessageType.Error);
                    if (GUILayout.Button("Fix it for me", GUILayout.Width(80)))
                        TextAnimatorSetupWindow.FixSettingsFileNotFound();
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (settingsObject == null) settingsObject = new SerializedObject(settings);

                    //Draws default settings instead
                    var databaseBehaviorsField = settingsObject.FindProperty(nameof(settings.behaviors))
                        .FindPropertyRelative(nameof(settings.behaviors.defaultDatabase));
                    var databaseAppearancesField = settingsObject.FindProperty(nameof(settings.appearances))
                        .FindPropertyRelative(nameof(settings.appearances.defaultDatabase));
                    var databaseActionsField = settingsObject.FindProperty(nameof(settings.actions))
                        .FindPropertyRelative(nameof(settings.actions.defaultDatabase));
                    DrawDatabaseField(ref editAppearances, "Appearances/Disappearances", databaseAppearancesField,
                        databaseAppearancesDrawer);
                    DrawDatabaseField(ref editBehaviors, "Behaviors", databaseBehaviorsField, databaseBehaviorsDrawer);
                    DrawDatabaseField(ref editActions, "Actions", databaseActionsField, databaseActionsDrawer);
                }
            }
            else
            {
                DrawDatabaseField(ref editAppearances, "Appearances/Disappearances", databaseAppearancesField,
                    databaseAppearancesDrawer);
                DrawDatabaseField(ref editBehaviors, "Behaviors", databaseBehaviorsField, databaseBehaviorsDrawer);
                DrawDatabaseField(ref editActions, "Actions", databaseActionsField, databaseActionsDrawer);
            }

            EditorGUI.indentLevel--;
        }

        #endregion
    }
}