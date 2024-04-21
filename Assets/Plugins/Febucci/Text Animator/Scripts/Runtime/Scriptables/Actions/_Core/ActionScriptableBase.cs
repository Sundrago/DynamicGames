using System;
using System.Collections;
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine;

namespace Febucci.UI.Actions
{
    [Serializable]
    public abstract class ActionScriptableBase : ScriptableObject, ITagProvider
    {
        [SerializeField] private string tagID;

        public string TagID
        {
            get => tagID;
            set => tagID = value;
        }

        public abstract IEnumerator DoAction(ActionMarker action, TypewriterCore typewriter, TypingInfo typingInfo);
    }
}