using System;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core
{
    //handles drawing of each single effect
    //with possibility to expand and directly draw the scriptable object as well
    [Serializable]
    internal class DatabaseSharedDrawer : SharedDrawer
    {
        [SerializeField] private AnimationElementDrawer[] elements;
        private SerializedProperty pairsProperty;

        protected override void OnEnabled(SerializedObject baseObject)
        {
            base.OnEnabled(baseObject);
            pairsProperty = baseObject.FindProperty("data");
            MatchEffectsWithArray();
        }

        private void MatchEffectsWithArray()
        {
            if (elements == null || elements.Length != pairsProperty.arraySize)
            {
                elements = new AnimationElementDrawer[pairsProperty.arraySize];
                for (var i = 0; i < elements.Length; i++)
                    elements[i] = new AnimationElementDrawer(pairsProperty.GetArrayElementAtIndex(i));
            }
        }

        protected override void _OnInspectorGUI()
        {
            MatchEffectsWithArray(); //putting this one here since might change after Undo

            for (var i = 0; i < elements.Length; i++)
            {
                var effect = elements[i];
                effect.Draw();
                if (effect.wantsToDelete >= 2)
                {
                    pairsProperty.DeleteArrayElementAtIndex(i);
                    MatchEffectsWithArray();
                    ApplyChanges();
                    return;
                }

                if (effect.somethingChanged)
                {
                    ApplyChanges();
                    effect.somethingChanged = false;
                }
            }

            //Adds new effect if there isn't any available slot already //TODO check every position
            if (elements.Length == 0 || elements[elements.Length - 1].hasScriptable)
                if (EditorGUILayout.Foldout(false, "->[Add new effect]", true))
                {
                    pairsProperty.InsertArrayElementAtIndex(pairsProperty.arraySize);
                    MatchEffectsWithArray();
                    //Sets last element as empty
                    elements[elements.Length - 1].propertyScriptable.objectReferenceValue = null;
                    ApplyChanges();
                    elements[elements.Length - 1].expanded = true;
                }
        }
    }
}