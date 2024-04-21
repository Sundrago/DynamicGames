using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    [CustomEditor(typeof(TextAnimatorInstallationData))]
    internal class TextAnimatorInstallationDataDrawer : Editor
    {
        private TextAnimatorInstallationData script;

        private void OnEnable()
        {
            script = (TextAnimatorInstallationData)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "This file is used by Text Animator to locate your installation path, so that you can move the folder anywhere you want, rename it and better organize your effects. Enjoy!",
                MessageType.None);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Installed version:");
            EditorGUILayout.LabelField(script.latestVersion);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (TextAnimatorSettings.Instance)
            {
                EditorGUILayout.HelpBox(
                    "If you wanted to edit the default settings instead, you can select the file via the button below.",
                    MessageType.None);
                if (GUILayout.Button("Select Settings")) Selection.activeObject = TextAnimatorSettings.Instance;
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "It seems you don't have a settings file, which is necessary for Text Animator. Do you want to fix it now?",
                    MessageType.Warning);
                if (GUILayout.Button("Fix it for me"))
                {
                    TextAnimatorSetupWindow.FixSettingsFileNotFound();
                    Selection.activeObject = TextAnimatorSettings.Instance;
                }
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Open About Window")) TextAnimatorSetupWindow.Menu_ShowWindowAlways();
        }
    }
}