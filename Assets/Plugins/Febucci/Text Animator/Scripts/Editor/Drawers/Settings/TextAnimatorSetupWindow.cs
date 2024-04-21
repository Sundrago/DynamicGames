using System;
using System.Linq;
using Febucci.UI.Actions;
using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    public class TextAnimatorSetupWindow : EditorWindow
    {
        private const string currentVersion = "2.0.2";
        private const string path_defaultInstallation = "Assets/Plugins/Febucci/Text Animator";

        private TextAnimatorInstallationData installationData;
        private bool settingsFileFound;


        /// <summary>
        ///     Called whenever the editor is loaded,
        ///     e.g. useful for checking if the asset is imported
        ///     for the first time or after an update
        /// </summary>
        [InitializeOnLoadMethod]
        internal static void TryShowingWindowOnLoad()
        {
            EditorApplication.delayCall += () => ShowWindow(true);
            ;
        }

        /// <summary>
        ///     Creates built-in effects databases and assigns it to the settings file as default.
        /// </summary>
        /// <remarks>
        ///     In case the settings file doesn't exist, it'll be created as well.
        ///     In case the built-in effects or databases already exist, they'll be overwritten.
        /// </remarks>
        public static void ResetToBuiltIn()
        {
            //makes sure the asset is installed
            GetOrCreateInstallationData();
            if (!TryGetInstallationFolder(out var installationFolder))
            {
                Debug.LogError("Something went wrong in locating TextAnimator's installation data.");
                return;
            }

            var settings = GetOrCreateSettings(installationFolder);
            CreateDefaultDatabases(installationFolder,
                out var beh,
                out var app,
                out var act);
            AssignDatabasesToSettings(settings, beh, app, act);
        }

        #region Updating to new versions

        /// <summary>
        ///     Checks for stuff and updates some project files if needed
        /// </summary>
        /// <param name="installationData"></param>
        private static void UpdateProject(TextAnimatorInstallationData installationData)
        {
            //TODO on new update
            installationData.latestVersion = currentVersion;
            EditorUtility.SetDirty(installationData);
        }

        #endregion

        #region Menu Items

        private const string menuParent = "Tools/Febucci/TextAnimator/";

        [MenuItem(menuParent + "About Window", priority = 1)]
        internal static void Menu_ShowWindowAlways()
        {
            ShowWindow(false);
        }

        [MenuItem(menuParent + "Utils/Select Settings SO", priority = 11)]
        private static void Menu_SelectSettingsScriptable()
        {
            if (TextAnimatorSettings.Instance)
            {
                Selection.activeObject = TextAnimatorSettings.Instance;
            }
            else
            {
                if (EditorUtility.DisplayDialog("Settings not found",
                        "Text Animator's settings file has not been found. Do you want to run the setup and create it automatically?",
                        "Yes", "No"))
                {
                    FixSettingsFileNotFound();
                    Selection.activeObject = TextAnimatorSettings.Instance;
                }
            }
        }

        #endregion

        #region Window

        private static void ShowWindow(bool onlyOnUpdate)
        {
            // already installed
            if (IsTextAnimatorInstalled(out var installationGUID))
            {
                var installationPath = AssetDatabase.GUIDToAssetPath(installationGUID);
                var installationData = AssetDatabase.LoadAssetAtPath<TextAnimatorInstallationData>(installationPath);

                // same version installed, no need to show the window
                if (onlyOnUpdate && installationData.latestVersion == currentVersion) return;

                //--- Updates to new version ---
                UpdateProject(installationData);
            }

            //--- First time import ---
            //Does nothing, asking the user to install with one click
            //Initializes the asset for the first time
            var window = (TextAnimatorSetupWindow)GetWindow(typeof(TextAnimatorSetupWindow), true,
                "Text Animator Setup", true);
            window.maxSize = new Vector2(351, 485);
            window.minSize = window.maxSize;
            window.settingsFileFound = TextAnimatorSettings.Instance;
        }

        private bool triedInstallingOnce;

        private void OnGUI()
        {
            if (!installationData)
            {
                if (!triedInstallingOnce)
                {
                    EditorGUILayout.LabelField("Installing....");
                    triedInstallingOnce = true;
                    installationData = GetOrCreateInstallationData();
                    Repaint();
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Unable to install package, please try to reopen this window from the Tools->Febucci menu");
                    return;
                }
            }

            //--- HEADER ---
            GUILayout.Box(TexturesLoader.AboutLogo, EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Welcome!", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Thank you for using Text Animator. Have fun bringing your projects to life!",
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();

            //--- VERSION STATUS ---

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Version:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(installationData.latestVersion, EditorStyles.whiteMiniLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (!settingsFileFound)
            {
                FixSettingsFileNotFound();
                settingsFileFound = true;
            }

            // --- LINKS etc. ---
            EditorGUILayout.LabelField("Online Resources", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Here are some useful resources.", EditorStyles.label);

            EditorGUILayout.BeginHorizontal();
            const string baseUrl = "https://febucci.com/text-animator-unity/";
            if (GUILayout.Button("What's New")) Application.OpenURL($"{baseUrl}changelog/");

            if (GUILayout.Button("Documentation")) Application.OpenURL($"{baseUrl}docs/");

            if (GUILayout.Button("Support")) Application.OpenURL($"{baseUrl}support/");

            EditorGUILayout.EndHorizontal();


            //--Extras--
            EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Would you like to be included in a future Text Animator showcase?",
                EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("-> Submit your game/project"))
                Application.OpenURL("https://www.febucci.com/text-animator-unity/showcase/");

            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField(
                "Please consider writing a review for the asset. It takes one minute but it really helps. Thanks!",
                EditorStyles.wordWrappedMiniLabel);
            if (GUILayout.Button("♥ Review on the Asset Store"))
                Application.OpenURL("https://assetstore.unity.com/packages/slug/158707");


            GUILayout.Space(5);
            EditorGUILayout.LabelField("Cheers! @febucci", EditorStyles.centeredGreyMiniLabel);
        }

        #endregion

        #region Installation

        private static TextAnimatorInstallationData GetOrCreateInstallationData()
        {
            if (IsTextAnimatorInstalled(out var installationGuid))
                return AssetDatabase.LoadAssetAtPath<TextAnimatorInstallationData>(
                    AssetDatabase.GUIDToAssetPath(installationGuid));

            var data = _CreateScriptableAssetAtPath<TextAnimatorInstallationData>(path_defaultInstallation + "/Data",
                "InstallationData");
            data.latestVersion = currentVersion;
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            return data;
        }

        private static bool IsTextAnimatorInstalled(out string installationGUID)
        {
            var path = AssetDatabase.FindAssets($"t:{nameof(TextAnimatorInstallationData)}");
            installationGUID = path.Length > 0 ? path[0] : string.Empty;
            return path.Length > 0;
        }

        private static bool TryGetInstallationFolder(out string result)
        {
            if (!IsTextAnimatorInstalled(out var installationGUID))
            {
                Debug.LogError("Unable to locate Text Animator's Installation file. Please re-run the setup.");
                result = null;
                return false;
            }

            result = AssetDatabase.GUIDToAssetPath(installationGUID);
            result = result.Substring(0, result.LastIndexOf('/'));
            return true;
        }

        #endregion

        #region Databases and Tags

        /// <summary>
        ///     Creates default effects and actions databases.
        /// </summary>
        /// <param name="installationFolder"></param>
        /// <param name="behaviors"></param>
        /// <param name="appearances"></param>
        /// <param name="actions"></param>
        /// <remarks>In case they already exist, they'll get overwritten.</remarks>
        private static void CreateDefaultDatabases(string installationFolder, out AnimationsDatabase behaviors,
            out AnimationsDatabase appearances, out ActionDatabase actions)
        {
            var progressTitle = "Text Animator";

            // --- DATABASES ---
            EditorUtility.DisplayProgressBar(progressTitle, "Creating Behaviors Database", 1 / 4f);
            behaviors = _CreateDatabase<AnimationsDatabase, AnimationScriptableBase>(installationFolder, "Behaviors",
                "Behaviors Database", EffectCategory.Behaviors);

            EditorUtility.DisplayProgressBar(progressTitle, "Creating Appearances Database", 2 / 4f);
            appearances = _CreateDatabase<AnimationsDatabase, AnimationScriptableBase>(installationFolder,
                "Appearances", "Appearances Database", EffectCategory.Appearances);

            EditorUtility.DisplayProgressBar(progressTitle, "Creating Actions Database", 3 / 4f);
            actions = _CreateDatabase<ActionDatabase, ActionScriptableBase>(installationFolder, "Actions",
                "Actions Database", EffectCategory.None);

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        public static DatabaseType _CreateDatabase<DatabaseType, ElementType>(string installationFolder,
            string folderName, string fileName, EffectCategory category)
            where DatabaseType : Database<ElementType> where ElementType : ScriptableObject, ITagProvider
        {
            var databasePath = installationFolder + $"/{folderName}";

            var database = _CreateScriptableAssetAtPath<DatabaseType>(databasePath, fileName);
            var types = GetScriptableElementsFromAssembly<ElementType>();
            for (var i = 0; i < types.Length; i++)
                if (TryCreatingDefaultTagScriptable(types[i], category, out var so))
                {
                    PlaceScriptableAtPath(so, databasePath);
                    database.Add(so as ElementType);
                }

            EditorUtility.SetDirty(database);
            var dat = new SerializedObject(database);
            dat.ApplyModifiedProperties();
            return database;
        }

        private static bool TryCreatingDefaultTagScriptable(Type type, EffectCategory category,
            out ScriptableObject result)
        {
            var attribute =
                type.GetCustomAttributes(typeof(TagInfoAttribute), true).FirstOrDefault() as TagInfoAttribute;
            if (attribute == null)
            {
                //skips default elements that do not have EffectInfo attribute
                result = default;
                return false;
            }

            //skips empty tags by design, e.g. Composite animation
            if (string.IsNullOrEmpty(attribute.tagID))
            {
                result = default;
                return false;
            }

            if (attribute is EffectInfoAttribute effectInfo)
                if (!effectInfo.category.HasFlag(category))
                {
                    result = default;
                    return false;
                }

            var so = CreateInstance(type);
            so.name = type.Name;

            //changes scriptable field based on default value attributes
            var defaultValueAttributes =
                type.GetCustomAttributes(typeof(DefaultValueAttribute), true) as DefaultValueAttribute[];

            var serializedSo = new SerializedObject(so);
            var tagID = serializedSo.FindProperty("tagID");

            if (defaultValueAttributes != null)
                foreach (var info in defaultValueAttributes)
                    serializedSo.FindProperty(info.variableName).floatValue = info.variableValue;

            tagID.stringValue = attribute.tagID;
            serializedSo.ApplyModifiedProperties();
            result = so;
            return true;
        }

        #endregion


        #region Settings

        private static void AssignDatabasesToSettings(TextAnimatorSettings settings,
            AnimationsDatabase behaviorsDatabase,
            AnimationsDatabase appearanceDatabase, ActionDatabase actionsDatabase)
        {
            var serialized = new SerializedObject(settings);
            serialized.FindProperty(nameof(settings.actions))
                .FindPropertyRelative(nameof(settings.actions.defaultDatabase)).objectReferenceValue = actionsDatabase;

            serialized.FindProperty(nameof(settings.behaviors))
                    .FindPropertyRelative(nameof(settings.behaviors.defaultDatabase)).objectReferenceValue =
                behaviorsDatabase;

            serialized.FindProperty(nameof(settings.appearances))
                    .FindPropertyRelative(nameof(settings.appearances.defaultDatabase)).objectReferenceValue =
                appearanceDatabase;

            serialized.ApplyModifiedProperties();
            serialized.Update();
        }


        private static TextAnimatorSettings GetOrCreateSettings(string installationFolder)
        {
            if (TextAnimatorSettings.Instance)
                return TextAnimatorSettings.Instance;

            return _CreateScriptableAssetAtPath<TextAnimatorSettings>(installationFolder + "/Resources",
                TextAnimatorSettings.expectedName);
        }


        /// <summary>
        ///     Creates a new settings file (with databases) inside the installation's Resources folder.
        ///     Also assigns default effects and actions databases,
        ///     either looking if they already exist or by creating new ones.
        /// </summary>
        /// <remarks>
        ///     P.S. A new settings file will be created even if it already exists but in another folder,
        ///     as the user might have moved it for archive/backups purposes.
        /// </remarks>
        public static void FixSettingsFileNotFound()
        {
            GetOrCreateInstallationData();
            if (!TryGetInstallationFolder(out var installationFolder))
            {
                Debug.LogError("Something went wrong in locating TextAnimator's installation data.");
                return;
            }

            DatabaseType GetOrCreateDatabase<DatabaseType, ElementType>(string folderName, string fileName,
                EffectCategory category)
                where DatabaseType : Database<ElementType> where ElementType : ScriptableObject, ITagProvider
            {
                var databaseGuid = AssetDatabase.FindAssets($"t:{nameof(DatabaseType)}").FirstOrDefault();

                if (!string.IsNullOrEmpty(databaseGuid))
                    return AssetDatabase.LoadAssetAtPath<DatabaseType>(AssetDatabase.GUIDToAssetPath(databaseGuid));

                // tries creating new database
                if (TryGetInstallationFolder(out installationFolder))
                    return _CreateDatabase<DatabaseType, ElementType>(installationFolder, folderName, fileName,
                        category);

                return null;
            }

            var settings = GetOrCreateSettings(installationFolder);
            AssignDatabasesToSettings(settings,
                GetOrCreateDatabase<AnimationsDatabase, AnimationScriptableBase>("Behaviors", "Behaviors Database",
                    EffectCategory.Behaviors),
                GetOrCreateDatabase<AnimationsDatabase, AnimationScriptableBase>("Appearances",
                    "Appearances Database", EffectCategory.Appearances),
                GetOrCreateDatabase<ActionDatabase, ActionScriptableBase>("Actions", "Actions Database",
                    EffectCategory.None));
        }

        #endregion

        #region Utilties

        public static void _CreateDirectoryIfDoesntExist(string path)
        {
            var folders = path.Split('/');
            var subPath = folders[0];
            for (var i = 1; i < folders.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(subPath + '/' + folders[i]))
                    AssetDatabase.CreateFolder(subPath, folders[i]);
                subPath += $"/{folders[i]}";
            }
        }

        private static Type[] GetScriptableElementsFromAssembly<T>() where T : ScriptableObject, ITagProvider
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(T).IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
                .Where(s => s.Assembly.FullName.StartsWith("Febucci.TextAnimator.Runtime"))
                .ToArray();
        }

        public static T _CreateScriptableAssetAtPath<T>(string path, string objectName) where T : ScriptableObject
        {
            var scriptable = CreateInstance<T>();
            scriptable.name = objectName;
            PlaceScriptableAtPath(scriptable, path);
            return scriptable;
        }

        private static void PlaceScriptableAtPath(ScriptableObject scriptable, string path)
        {
            _CreateDirectoryIfDoesntExist(path);
            AssetDatabase.CreateAsset(scriptable, path + $"/{scriptable.name}.asset");
        }

        #endregion
    }
}