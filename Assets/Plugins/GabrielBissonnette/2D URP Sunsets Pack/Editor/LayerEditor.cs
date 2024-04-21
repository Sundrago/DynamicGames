using UnityEditor;

[CustomEditor(typeof(LayerManager))]
public class LayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var layerManager = (LayerManager)target;

        layerManager.ChangeColor();
    }
}