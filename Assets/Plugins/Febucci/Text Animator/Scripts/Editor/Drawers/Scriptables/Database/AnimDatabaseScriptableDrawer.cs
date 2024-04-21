using Febucci.UI.Effects;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(AnimationsDatabase), true)]
    internal class AnimDatabaseScriptableDrawer : Editor
    {
        private readonly DatabaseSharedDrawer drawer = new();

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}