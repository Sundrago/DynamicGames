using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayerManager))]
public class LayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LayerManager layerManager = (LayerManager)target;

        layerManager.ChangeColor();
    }
}
