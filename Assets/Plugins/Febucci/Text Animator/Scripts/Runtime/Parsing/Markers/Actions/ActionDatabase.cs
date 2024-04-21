using System;
using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Actions
{
    [Serializable]
    [CreateAssetMenu(fileName = "ActionDatabase", menuName = "Text Animator/Actions/Create Actions Database",
        order = 100)]
    public class ActionDatabase : Database<ActionScriptableBase>
    {
    }
}