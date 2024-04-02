using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Games.Land
{
    /// <summary>
    /// Manages and holds the stages of the game.
    /// </summary>
    public class StageManager : MonoBehaviour
    {
        public List<StageItemHolder> stages;

#if UNITY_EDITOR
        [Button]
        private void AddAllItemsToList()
        {
            stages = new List<StageItemHolder>(GetComponentsInChildren<StageItemHolder>());
        }
#endif
    }
}