using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    /// <summary>
    ///     Applies multiples animations, allowing user to use one tag for all of them
    /// </summary>
    [Preserve]
    [CreateAssetMenu(fileName = "Composite Animation", menuName = "Text Animator/Animations/Special/Composite")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class CompositeAnimation : AnimationScriptableBase
    {
        public AnimationScriptableBase[] animations = new AnimationScriptableBase[0];

        private void OnValidate()
        {
            ValidateArray();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            ValidateArray();

            foreach (var anim in animations) anim.InitializeOnce();
        }

        public override void ResetContext(TAnimCore animator)
        {
            foreach (var anim in animations) anim.ResetContext(animator);
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            base.SetModifier(modifier);
            foreach (var anim in animations) anim.SetModifier(modifier);
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            foreach (var anim in animations)
                if (anim.CanApplyEffectTo(character, animator))
                    anim.ApplyEffectTo(ref character, animator);
        }

        //Prevents double check
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator)
        {
            return true;
        }

        public override float GetMaxDuration()
        {
            //Calculates max duration between animations
            float maxDuration = -1;
            foreach (var anim in animations) maxDuration = Mathf.Max(maxDuration, anim.GetMaxDuration());

            return maxDuration;
        }

        private void ValidateArray()
        {
            //prevents recursion
            var validated = new List<AnimationScriptableBase>();

            for (var i = 0; i < animations.Length; i++)
                if (animations[i] != this)
                    validated.Add(animations[i]);

            animations = validated.ToArray();
        }
    }
}