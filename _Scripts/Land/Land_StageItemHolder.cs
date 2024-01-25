using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Land_StageItemHolder : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    
    #if UNITY_EDITOR
    [Button]
    private void AddAllItemsToList()
    {
        items = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            items.Add(transform.GetChild(i).gameObject);
        }
    }
    #endif
}
