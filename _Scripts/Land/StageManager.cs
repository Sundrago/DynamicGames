using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StageManager : MonoBehaviour
{
    public List<GameObject> stages = new List<GameObject>();
    [SerializeField] GameObject ending;
    
    #if UNITY_EDITOR
    [Button]
    private void AddAllItemsToList()
    {
        stages = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            stages.Add(transform.GetChild(i).gameObject);
        }
    }
    #endif
}
