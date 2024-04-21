using UnityEditor;
using UnityEngine;

namespace AssetKits.ParticleImage.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ParticleTrailRenderer))]
    public class TrailRendererEditor : UnityEditor.Editor
    {
        private void Awake()
        {
            MonoScript.FromMonoBehaviour(target as ParticleTrailRenderer)
                .SetIcon(Resources.Load<Texture2D>("TrailIcon"));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Particle Image trail renderer.", MessageType.Info);
        }
    }
}