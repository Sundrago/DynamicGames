using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Utility : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> setActiveGameobjects;

#if UNITY_EDITOR
    [Button]
    void SetActiveTrue()
    {
        foreach (GameObject obj in setActiveGameobjects)
        {
            obj.SetActive(true);
        }
    }
    
    [Button]
    void SetActiveFalse()
    {
        foreach (GameObject obj in setActiveGameobjects)
        {
            obj.SetActive(false);
        }
    }

    [Button]
    void ResetPlayerPrefData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
#endif
}
