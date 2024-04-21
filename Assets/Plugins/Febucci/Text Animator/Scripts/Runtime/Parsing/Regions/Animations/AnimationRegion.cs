using System;
using System.Text;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Core.Parsing
{
    [Flags]
    public enum VisibilityMode
    {
        OnVisible = 1,
        OnHiding = 2,
        Persistent = OnVisible | OnHiding
    }

    /// <summary>
    ///     Contains information of a region in the text
    /// </summary>
    public class AnimationRegion : RegionBase
    {
        public readonly AnimationScriptableBase animation;
        private readonly VisibilityMode visibilityMode;

        public AnimationRegion(string tagId, VisibilityMode visibilityMode, AnimationScriptableBase animation) :
            base(tagId)
        {
            this.visibilityMode = visibilityMode;
            this.animation = animation;
        }

        public bool IsVisibilityPolicySatisfied(bool visible)
        {
            return visibilityMode == VisibilityMode.Persistent ||
                   visibilityMode.HasFlag(VisibilityMode.OnVisible) == visible;
        }

        #region Animation

        public virtual void SetupContextFor(TAnimCore animator, ModifierInfo[] modifiers)
        {
            animation.ResetContext(animator);

            foreach (var mod in modifiers)
                animation.SetModifier(mod);
        }

        #endregion

        public override string ToString()
        {
            var text = new StringBuilder();
            text.Append("tag: ");
            text.Append(tagId);
            if (ranges.Length == 0) text.Append("\nNo ranges");
            else
                for (var i = 0; i < ranges.Length; i++)
                {
                    text.Append('\n');
                    text.Append('-');
                    text.Append('-');
                    text.Append(ranges[i]);
                }

            return text.ToString();
        }

        #region Ranges

        public void OpenNewRange(int startIndex)
        {
            OpenNewRange(startIndex, Array.Empty<string>());
        }

        public void OpenNewRange(int startIndex, string[] tagWords)
        {
            Array.Resize(ref ranges, ranges.Length + 1);
            var range = new TagRange(new Vector2Int(startIndex, int.MaxValue));

            //Adds modifiers
            for (var i = 1; i < tagWords.Length; i++) //starts from 1 'cos skips tag name
            {
                var tag = tagWords[i];
                var equalIndex = tag.IndexOf('=');
                if (equalIndex <= 0) continue; //invalid modifier

                if (FormatUtils.TryGetFloat(tag.Substring(equalIndex + 1), 0, out var result))
                {
                    Array.Resize(ref range.modifiers, range.modifiers.Length + 1);
                    range.modifiers[range.modifiers.Length - 1] =
                        new ModifierInfo(tag.Substring(0, equalIndex), result);
                }
            }

            ranges[ranges.Length - 1] = range;
        }

        //TODO testing
        public void TryClosingRange(int endIndex)
        {
            if (ranges.Length == 0) return; //no otherTag was opened before

            for (var i = ranges.Length - 1; i >= 0; i--)
            {
                if (ranges[i].indexes.y != int.MaxValue) continue; // otherTag was already closed

                var range = ranges[i];
                range.indexes.y = endIndex;
                ranges[i] = range;
                break; //found a range to close
            }
        }

        public void CloseAllOpenedRanges(int endIndex)
        {
            if (ranges.Length == 0) return; //no otherTag was opened before

            for (var i = ranges.Length - 1; i >= 0; i--)
            {
                if (ranges[i].indexes.y != int.MaxValue) continue; // otherTag was already closed

                var range = ranges[i];
                range.indexes.y = endIndex;
                ranges[i] = range;
            }
        }

        #endregion
    }
}