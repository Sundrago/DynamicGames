using UnityEditor;
using UnityEngine;

namespace Febucci.Attributes
{
    [CustomPropertyDrawer(typeof(PositiveValueAttribute))]
    public class PositiveValueAttributeDrawer : PropertyDrawer
    {
        private const float minValue = .01f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    var intValue = property.intValue;
                    intValue = EditorGUI.IntField(position, label, intValue);
                    if (intValue >= minValue)
                        property.intValue = intValue;
                    break;

                case SerializedPropertyType.Float:
                    var floatValue = property.floatValue;
                    floatValue = EditorGUI.FloatField(position, label, floatValue);

                    property.floatValue = Mathf.Clamp(floatValue, minValue, floatValue);
                    break;

                case SerializedPropertyType.Vector2:
                    var vecValue = property.vector2Value;
                    vecValue = EditorGUI.Vector2Field(position, label, vecValue);

                    vecValue.x = Mathf.Clamp(vecValue.x, minValue, vecValue.x);
                    vecValue.y = Mathf.Clamp(vecValue.y, minValue, vecValue.y);

                    property.vector2Value = vecValue;
                    break;

                default:
                    base.OnGUI(position, property, label);
                    break;
            }
        }
    }
}