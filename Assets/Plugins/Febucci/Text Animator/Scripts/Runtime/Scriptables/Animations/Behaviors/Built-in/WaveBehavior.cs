using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Wave", fileName = "Wave Behavior")]
    [EffectInfo("wave", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 7.27f)]
    [DefaultValue(nameof(baseFrequency), 4f)]
    [DefaultValue(nameof(baseWaveSize), .4f)]
    public sealed class WaveBehavior : BehaviorScriptableSine
    {
        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.MoveChar(
                Vector3.up * Mathf.Sin(animator.time.timeSinceStart * frequency + character.index * waveSize)
                           * amplitude * character.uniformIntensity);
        }
    }
}