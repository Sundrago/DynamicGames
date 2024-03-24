using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Build_StageManager : MonoBehaviour
{
    public List<Land_StageItemHolder> stages = new List<Land_StageItemHolder>();
    
    #if UNITY_EDITOR
    [Button]
    private void AddAllItemsToList()
    {
        stages = PopulateItemsFromChildren();
    }

    private List<Land_StageItemHolder> PopulateItemsFromChildren()
    {
        List<Land_StageItemHolder> updatedStages = new List<Land_StageItemHolder>();

        for (int i = 0; i < transform.childCount; i++)
        {
            updatedStages.Add(transform.GetChild(i).gameObject.GetComponent<Land_StageItemHolder>());
        }

        return updatedStages;
    }
    #endif
}
