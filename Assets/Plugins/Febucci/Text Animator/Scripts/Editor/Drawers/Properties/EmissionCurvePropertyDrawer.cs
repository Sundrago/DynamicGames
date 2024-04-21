using Febucci.UI.Effects;
using UnityEditor;
using UnityEngine;

namespace Febucci.UI.Core
{
    [CustomPropertyDrawer(typeof(EmissionCurveProperty))]
    public class EmissionCurvePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lineByLine = position;
            lineByLine.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(lineByLine, property.isExpanded, label, true);
            if (!property.isExpanded) return;

            // -- inner area smaller ---
            position.y = lineByLine.yMax;
            position.height -= lineByLine.height;
            position.x += 15;
            position.width -= 15;

            lineByLine.x = position.x;
            lineByLine.width = position.width;
            lineByLine.y += lineByLine.height;

            // -- cycles --
            var cycles = property.FindPropertyRelative(nameof(EmissionCurve.cycles));
            var half = lineByLine;
            half.width /= 2f;
            EditorGUI.PropertyField(half, cycles);
            half.x += half.width + 5;
            half.width -= 5;
            EditorGUI.LabelField(half, cycles.intValue > 0 ? "cycles until end" : "effect is infinite");

            lineByLine.y += lineByLine.height;
            EditorGUI.PropertyField(lineByLine, property.FindPropertyRelative(nameof(EmissionCurve.duration)));
            lineByLine.y += lineByLine.height;
            EditorGUI.PropertyField(lineByLine, property.FindPropertyRelative(nameof(EmissionCurve.weightOverTime)));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded
                ? EditorGUIUtility.singleLineHeight * 4
                : EditorGUIUtility.singleLineHeight;
        }
    }
}