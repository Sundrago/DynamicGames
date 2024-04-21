using System;

namespace Febucci.UI.Core
{
    [Flags]
    public enum EffectCategory
    {
        None = 0,
        Behaviors = 1,
        Appearances = 2,
        All = Behaviors | Appearances
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TagInfoAttribute : Attribute
    {
        public readonly string tagID;

        public TagInfoAttribute(string tagID)
        {
            this.tagID = tagID;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class EffectInfoAttribute : TagInfoAttribute
    {
        public readonly EffectCategory category;

        public EffectInfoAttribute(string tagID, EffectCategory category) : base(tagID)
        {
            this.category = category;
        }
    }
}