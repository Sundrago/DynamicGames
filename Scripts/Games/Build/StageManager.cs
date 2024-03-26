using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Games.Build
{
    /// <summary>
    ///     Class responsible for managing the stages in the game.
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        public List<Land_StageItemHolder> stages = new();

#if UNITY_EDITOR
        [Button]
        private void AddAllItemsToList()
        {
            stages = PopulateItemsFromChildren();
        }

        private List<Land_StageItemHolder> PopulateItemsFromChildren()
        {
            var updatedStages = new List<Land_StageItemHolder>();

            for (var i = 0; i < transform.childCount; i++)
                updatedStages.Add(transform.GetChild(i).gameObject.GetComponent<Land_StageItemHolder>());

            return updatedStages;
        }
#endif
    }
}