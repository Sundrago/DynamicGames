using System;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core
{
    [Serializable]
    internal class AnimationElementDrawer
    {
        public SerializedProperty propertyScriptable;
        public bool expanded;
        public bool somethingChanged;
        public int wantsToDelete;
        private GenericSharedDrawer drawer;
        private GUIContent scriptableNameContent;

        public AnimationElementDrawer(SerializedProperty propertyArrayElementPair)
        {
            propertyScriptable = propertyArrayElementPair;
            drawer = new GenericSharedDrawer(false);
            expanded = false;
            wantsToDelete = 0;
            scriptableNameContent = new GUIContent("Scriptable");
        }

        public bool hasScriptable => propertyScriptable.objectReferenceValue != null;

        public void Draw()
        {
            somethingChanged = false;
            var drawWarning = false;
            string foldoutName;
            if (propertyScriptable.objectReferenceValue is ITagProvider tag)
            {
                if (string.IsNullOrEmpty(tag.TagID))
                {
                    drawWarning = true;
                    foldoutName = "[!] Empty Tag";
                }
                else
                {
                    foldoutName = tag.TagID;
                }
            }
            else
            {
                drawWarning = true;
                foldoutName = "[!] Empty Slot";
            }

            EditorGUILayout.BeginHorizontal();
            expanded = EditorGUILayout.Foldout(expanded, foldoutName, true);
            GUI.backgroundColor = wantsToDelete == 1 ? Color.red : Color.white;
            GUI.enabled = expanded;
            if (GUILayout.Button(wantsToDelete == 1 ? "Confirm?" : "Delete", EditorStyles.helpBox, GUILayout.Width(55)))
                wantsToDelete++;
            GUI.enabled = true;
            if (!expanded)
                wantsToDelete = 0;

            GUI.backgroundColor = Color.white;

            if (drawWarning)
                EditorGUILayout.HelpBox("Invalid", MessageType.Warning);
            EditorGUILayout.EndHorizontal();

            if (expanded)
            {
                EditorGUI.indentLevel++;
                DrawInfo();
                DrawBody();
                EditorGUI.indentLevel--;
            }
        }


        private void DrawInfo()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propertyScriptable, scriptableNameContent);
            if (EditorGUI.EndChangeCheck())
            {
                //refreshes drawer
                somethingChanged = true;
                drawer = new GenericSharedDrawer(false);

                if (propertyScriptable.serializedObject.hasModifiedProperties)
                    propertyScriptable.serializedObject.ApplyModifiedProperties();

                expanded = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBody()
        {
            drawer.OnInspectorGUI(propertyScriptable);
        }
    }
}