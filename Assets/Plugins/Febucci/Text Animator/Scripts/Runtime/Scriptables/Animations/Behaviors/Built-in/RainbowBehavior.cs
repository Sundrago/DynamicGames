using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Rainbow", fileName = "Rainbow Behavior")]
    [EffectInfo("rainb", EffectCategory.Behaviors)]
    public sealed class RainbowBehavior : BehaviorScriptableBase
    {
        public float baseFrequency = 0.5f;
        public float baseWaveSize = 0.08f;


        private float frequency;

        private Color32 temp;
        private float waveSize;

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                //frequency
                case "f":
                    frequency = baseFrequency * modifier.value;
                    break;
                //wave size
                case "s":
                    waveSize = baseWaveSize * modifier.value;
                    break;
            }
        }

        public override void ResetContext(TAnimCore animator)
        {
            frequency = baseFrequency;
            waveSize = baseWaveSize;
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            for (byte i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                //shifts hue
                temp = Color.HSVToRGB(
                    Mathf.PingPong(animator.time.timeSinceStart * frequency + character.index * waveSize, 1), 1, 1);
                temp.a = character.current.colors[i].a; //preserves original alpha
                character.current.colors[i] = temp;
            }
        }
    }
}