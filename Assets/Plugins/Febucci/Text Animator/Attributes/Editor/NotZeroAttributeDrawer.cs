using UnityEditor;
using UnityEngine;

namespace Febucci.Attributes
{
    [CustomPropertyDrawer(typeof(NotZeroAttribute))]
    public class NotZeroAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    var intValue = property.intValue;
                    intValue = EditorGUI.IntField(position, label, intValue);
                    if (intValue != 0)
                        property.intValue = intValue;
                    break;

                case SerializedPropertyType.Float:
                    var floatValue = property.floatValue;
                    floatValue = EditorGUI.FloatField(position, label, floatValue);

                    if (floatValue != 0)
                        property.floatValue = floatValue;

                    break;

                case SerializedPropertyType.Vector2:
                    var vecValue = property.vector2Value;
                    vecValue = EditorGUI.Vector2Field(position, label, vecValue);

                    property.vector2Value = new Vector2(
                        vecValue.x != 0 || vecValue.y != 0 ? vecValue.x : property.vector2Value.x,
                        vecValue.y != 0 || vecValue.x != 0 ? vecValue.y : property.vector2Value.y);

                    break;


                default:
                    base.OnGUI(position, property, label);
                    break;
            }
        }
    }
}