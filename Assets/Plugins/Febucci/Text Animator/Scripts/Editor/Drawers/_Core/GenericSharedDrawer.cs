using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core
{
    /// <summary>
    ///     Could draw anything
    /// </summary>
    internal class GenericSharedDrawer : SharedDrawer
    {
        private readonly bool showScript;

        public GenericSharedDrawer(bool showScript)
        {
            this.showScript = showScript;
        }

        protected override void _OnInspectorGUI()
        {
            //draws every visible property (first level children only) of the base serialized object
            var iterator = baseObject.GetIterator();
            iterator.NextVisible(true);
            do
            {
                if (iterator.name == "m_Script")
                {
                    if (showScript)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(iterator, true);
                        GUI.enabled = true;
                    }

                    continue;
                }

                if (iterator.isArray)
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    //if (iterator.displayName.Length > 0)
                    //EditorGUILayout.PrefixLabel(iterator.displayName.Replace("Base ", ""));
                    EditorGUILayout.PropertyField(iterator, true);
                    //EditorGUILayout.PropertyField(iterator, GUIContent.none, true);
                    EditorGUILayout.EndHorizontal();
                }
            } while (iterator.NextVisible(false));
        }
    }
}