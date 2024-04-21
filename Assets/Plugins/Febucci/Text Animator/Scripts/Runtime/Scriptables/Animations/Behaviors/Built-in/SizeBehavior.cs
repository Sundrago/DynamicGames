using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Size", fileName = "Size Behavior")]
    [EffectInfo("incr", EffectCategory.Behaviors)]
    public sealed class SizeBehavior : BehaviorScriptableBase
    {
        public float baseAmplitude = 1.5f;
        public float baseFrequency = 4;
        public float baseWaveSize = 0.2f;

        private float amplitude;
        private float frequency;
        private float waveSize;

        public override void ResetContext(TAnimCore animator)
        {
            amplitude = baseAmplitude * -1 + 1;
            frequency = baseFrequency;
            waveSize = baseWaveSize;
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a":
                    amplitude = baseAmplitude * modifier.value * -1 + 1;
                    break;
                case "f":
                    frequency = baseFrequency * modifier.value;
                    break;
                case "w":
                    waveSize = baseWaveSize * modifier.value;
                    break;
            }
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.LerpUnclamped(
                character.current.positions.GetMiddlePos(),
                (Mathf.Cos(animator.time.timeSinceStart * frequency + character.index * waveSize) + 1) / 2f *
                amplitude);
        }
    }
}