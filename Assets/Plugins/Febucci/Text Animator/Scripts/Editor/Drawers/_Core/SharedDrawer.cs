using System;
using UnityEditor;

namespace Febucci.UI.Core
{
    /// <summary>
    ///     Base class that can be used to create drawers used by multiple
    ///     custom editors, both components and scriptable objects
    /// </summary>
    [Serializable]
    internal abstract class SharedDrawer
    {
        protected SerializedObject baseObject;
        protected SerializedProperty baseProperty;
        private bool initialized;

        protected virtual void OnEnabled(SerializedObject baseObject)
        {
        }

        public void OnInspectorGUI(SerializedProperty baseProperty)
        {
            if (baseProperty == null) return;
            if (baseProperty.objectReferenceValue == null) return;

            if (baseProperty != this.baseProperty)
                initialized = false;

            if (!initialized)
            {
                this.baseProperty = baseProperty;
                //hacky unity way to reach the actual serialized object
                baseObject = new SerializedObject(baseProperty.objectReferenceValue);
                OnEnabled(baseObject);
                initialized = true;
            }

            baseObject.Update();
            _OnInspectorGUI();

            ApplyChanges();
        }

        public void OnInspectorGUI(SerializedObject baseObject)
        {
            if (baseObject == null) return;
            if (this.baseObject != baseObject)
                initialized = false;

            if (!initialized)
            {
                this.baseObject = baseObject;
                OnEnabled(baseObject);
                initialized = true;
            }

            baseObject.Update();
            _OnInspectorGUI();

            ApplyChanges();
        }

        protected virtual void _OnInspectorGUI()
        {
        }

        protected void ApplyChanges()
        {
            if (baseObject.hasModifiedProperties)
                baseObject.ApplyModifiedProperties();
        }
    }
}