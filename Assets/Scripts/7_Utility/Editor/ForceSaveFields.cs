using UnityEditor;

namespace Utility.Editor
{
    internal class ForceSaveFields : UnityEditor.Editor
    {
        [MenuItem("Tools/Force Reserialize Assets")]
        private static void ForceReserialzed()
        {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}