using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Build
{
    /// <summary>
    ///     Represents a holder for currentStageIdx items.
    /// </summary>
    public class StageItemHolder : MonoBehaviour
    {
        public List<GameObject> items = new();

#if UNITY_EDITOR
        [Button]
        private void AddAllItemsToList()
        {
            items = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++) items.Add(transform.GetChild(i).gameObject);
        }
#endif
    }
}