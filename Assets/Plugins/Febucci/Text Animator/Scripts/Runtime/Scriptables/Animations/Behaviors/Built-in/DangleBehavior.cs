using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Dangle", fileName = "Dangle Behavior")]
    [EffectInfo("dangle", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 7.87f)]
    [DefaultValue(nameof(baseFrequency), 3.37f)]
    [DefaultValue(nameof(baseWaveSize), .306f)]
    public sealed class DangleBehavior : BehaviorScriptableSine
    {
        public bool anchorBottom;
        private float sin;

        private int targetIndex1;
        private int targetIndex2;

        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);

            //bottom
            if (anchorBottom)
            {
                targetIndex1 = 1;
                targetIndex2 = 2;
            }
            else
            {
                targetIndex1 = 0;
                targetIndex2 = 3;
            }
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            sin = Mathf.Sin(frequency * animator.time.timeSinceStart + character.index * waveSize) * amplitude *
                  character.uniformIntensity;

            //moves one side (top or bottom) torwards one direction
            character.current.positions[targetIndex1] += Vector3.right * sin;
            character.current.positions[targetIndex2] += Vector3.right * sin;
        }
    }
}