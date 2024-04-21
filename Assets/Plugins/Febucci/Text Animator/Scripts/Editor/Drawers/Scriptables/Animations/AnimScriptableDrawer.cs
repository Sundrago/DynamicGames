using Febucci.UI.Effects;
using UnityEditor;

namespace Febucci.UI.Core
{
    [CustomEditor(typeof(AnimationScriptableBase), true)]
    internal class AnimScriptableDrawer : Editor
    {
        private readonly GenericSharedDrawer drawer = new(true);

        public override void OnInspectorGUI()
        {
            drawer.OnInspectorGUI(serializedObject);
        }
    }
}