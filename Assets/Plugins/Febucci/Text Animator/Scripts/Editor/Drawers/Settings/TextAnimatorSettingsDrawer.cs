using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    [CustomEditor(typeof(TextAnimatorSettings))]
    public class TextAnimatorSettingsDrawer : Editor
    {
        private bool extraSettings;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            extraSettings = EditorGUILayout.Foldout(extraSettings, "Extra Settings", EditorStyles.foldoutHeader);
            if (extraSettings)
                if (GUILayout.Button("Reset Default Effects and Actions"))
                    if (EditorUtility.DisplayDialog("Text Animator",
                            "Are you sure you want to reset the default effects and actions?", "Yes", "No"))
                        TextAnimatorSetupWindow.ResetToBuiltIn();
        }
    }
}