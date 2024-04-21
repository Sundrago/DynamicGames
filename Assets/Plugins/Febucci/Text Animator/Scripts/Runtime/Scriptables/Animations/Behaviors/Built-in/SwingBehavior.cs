using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Swing", fileName = "Swing Behavior")]
    [EffectInfo("swing", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 22.74f)]
    [DefaultValue(nameof(baseFrequency), 3.65f)]
    [DefaultValue(nameof(baseWaveSize), .171f)]
    public sealed class SwingBehavior : BehaviorScriptableSine
    {
        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.RotateChar(
                Mathf.Cos(animator.time.timeSinceStart * frequency + character.index * waveSize) * amplitude);
        }
    }
}