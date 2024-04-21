using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(fileName = "Size Appearance", menuName = "Text Animator/Animations/Appearances/Size")]
    [EffectInfo("size", EffectCategory.Appearances)]
    public sealed class SizeAppearance : AppearanceScriptableBase
    {
        public float baseAmplitude = 2;
        private float amplitude;

        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);
            amplitude = baseAmplitude * -1 + 1;
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.LerpUnclamped(
                character.current.positions.GetMiddlePos(),
                Tween.EaseIn(1 - character.passedTime / duration) * amplitude
            );
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a":
                    amplitude = baseAmplitude * modifier.value;
                    break;
                default:
                    base.SetModifier(modifier);
                    break;
            }
        }
    }
}