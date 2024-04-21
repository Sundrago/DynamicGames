using Febucci.UI.Actions;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(ActionScriptableBase), true)]
    internal class ActionScriptableDrawer : Editor
    {
        private readonly GenericSharedDrawer drawer = new(true);

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}