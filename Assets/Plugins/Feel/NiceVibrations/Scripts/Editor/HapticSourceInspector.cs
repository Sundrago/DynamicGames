// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEditor;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    [CustomEditor(typeof(HapticSource))]
    [CanEditMultipleObjects]
    /// <summary>
    /// Provides an inspector for the HapticSource component
    /// </summary>
    ///
    /// The inspector lets you link a HapticSource to a HapticClip.
    public class HapticSourceInspector : Editor
    {
        public static GUIContent hapticClipLabel =
            EditorGUIUtility.TrTextContent("Haptic Clip", "The HapticClip asset played by the HapticSource.");

        public static GUIContent fallbackPresetLabel = EditorGUIUtility.TrTextContent("Haptic Preset fallback",
            "Set the haptic preset to play in case the device doesn't support playback of haptic clips");

        public static GUIContent loopLabel =
            EditorGUIUtility.TrTextContent("Loop", "Set the haptic source to loop playback of the haptic clip");

        private SerializedProperty fallbackPreset;
        private SerializedProperty frequencyShift;

        private SerializedProperty hapticClip;
        private string hapticsDirectory;
        private SerializedProperty level;
        private SerializedProperty loop;
        private SerializedProperty priority;

        private void OnEnable()
        {
            hapticClip = serializedObject.FindProperty("clip");
            priority = serializedObject.FindProperty("priority");
            level = serializedObject.FindProperty("_level");
            frequencyShift = serializedObject.FindProperty("_frequencyShift");
            fallbackPreset = serializedObject.FindProperty("_fallbackPreset");
            loop = serializedObject.FindProperty("_loop");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(hapticClip, hapticClipLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(fallbackPreset, fallbackPresetLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(loop, loopLabel);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            CreatePrioritySlider();
            CreateLevelSlider();
            CreateFrequencyShiftSlider();

            serializedObject.ApplyModifiedProperties();
        }

        /// Helper function to create a priority slider for haptic source with High and Max text labels.
        private void CreatePrioritySlider()
        {
            var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            EditorGUI.IntSlider(position, priority, 0, 256);

            // Move to next line
            position.y += EditorGUIUtility.singleLineHeight;

            // Subtract the label
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            // Subtract the text field width thats drawn with slider
            position.width -= EditorGUIUtility.fieldWidth;

            var style = GUI.skin.label;
            var defaultAlignment = GUI.skin.label.alignment;
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(position, "High", style);
            style.alignment = TextAnchor.UpperRight;
            EditorGUI.LabelField(position, "Low", style);
            GUI.skin.label.alignment = defaultAlignment;

            // Allow space for the High/Low labels
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        /// Helper function to create a level slider for haptic
        /// source with labels.
        private void CreateLevelSlider()
        {
            var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            EditorGUI.Slider(position, level, 0.0f, 5.0f);

            // Move to next line
            position.y += EditorGUIUtility.singleLineHeight;

            // Subtract the label
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            // Subtract the text field width thats drawn with slider
            position.width -= EditorGUIUtility.fieldWidth;

            var style = GUI.skin.label;
            var defaultAlignment = GUI.skin.label.alignment;
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(position, "0.0", style);
            style.alignment = TextAnchor.UpperRight;
            EditorGUI.LabelField(position, "5.0", style);
            GUI.skin.label.alignment = defaultAlignment;

            // Allow space for the labels
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        /// Helper function to create a frequency shift slider for haptic
        /// source with labels.
        private void CreateFrequencyShiftSlider()
        {
            var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            EditorGUI.Slider(position, frequencyShift, -1.0f, 1.0f);

            // Move to next line
            position.y += EditorGUIUtility.singleLineHeight;

            // Subtract the label
            position.x += EditorGUIUtility.labelWidth;
            position.width -= EditorGUIUtility.labelWidth;

            // Subtract the text field width thats drawn with slider
            position.width -= EditorGUIUtility.fieldWidth;

            var style = GUI.skin.label;
            var defaultAlignment = GUI.skin.label.alignment;
            style.alignment = TextAnchor.UpperLeft;
            EditorGUI.LabelField(position, "-1.0", style);
            style.alignment = TextAnchor.UpperRight;
            EditorGUI.LabelField(position, "1.0", style);
            GUI.skin.label.alignment = defaultAlignment;

            // Allow space for the labels
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
    }
}