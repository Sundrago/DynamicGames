using System;
using UnityEngine;

namespace Febucci.UI
{
    [Serializable]
    internal class TextAnimatorInstallationData : ScriptableObject
    {
        [SerializeField] internal string latestVersion = "None"; //stores the latest version
    }
}