using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Build
{
    /// <summary>
    ///     Responsible for managing the stages in the build game.
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        public List<StageItemHolder> stages = new();

#if UNITY_EDITOR
        [Button]
        private void AddAllItemsToList()
        {
            stages = PopulateItemsFromChildren();
        }

        private List<StageItemHolder> PopulateItemsFromChildren()
        {
            var updatedStages = new List<StageItemHolder>();

            for (var i = 0; i < transform.childCount; i++)
                updatedStages.Add(transform.GetChild(i).gameObject.GetComponent<StageItemHolder>());

            return updatedStages;
        }
#endif
    }
}