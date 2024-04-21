using Febucci.UI.Actions;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(ActionDatabase), true)]
    internal class ActionDatabaseScriptableDrawer : Editor
    {
        private readonly DatabaseSharedDrawer drawer = new();

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}