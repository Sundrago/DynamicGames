using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    public class SceneUtility : MonoBehaviour
    {
        [SerializeField] private List<GameObject> setActiveGameobjects;

#if UNITY_EDITOR
        [Button]
        private void SetActiveTrue()
        {
            foreach (var obj in setActiveGameobjects) obj.SetActive(true);
        }

        [Button]
        private void SetActiveFalse()
        {
            foreach (var obj in setActiveGameobjects) obj.SetActive(false);
        }

        [Button]
        private void ResetPlayerPrefData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
#endif
    }
}