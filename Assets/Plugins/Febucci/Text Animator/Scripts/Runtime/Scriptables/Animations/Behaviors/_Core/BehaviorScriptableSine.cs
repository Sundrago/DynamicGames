using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    public abstract class BehaviorScriptableSine : BehaviorScriptableBase
    {
        //--- BASE VALUES ---
        public float baseAmplitude = 1;
        public float baseFrequency = 1;
        [Range(0, 1)] public float baseWaveSize = .2f;

        //--- EFFECTS INFO ---
        protected float amplitude;
        protected float frequency;
        protected float waveSize;


        public override void ResetContext(TAnimCore animator)
        {
            amplitude = baseAmplitude;
            frequency = baseFrequency;
            waveSize = baseWaveSize;
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a":
                    amplitude = baseAmplitude * modifier.value;
                    break;
                case "f":
                    frequency = baseFrequency * modifier.value;
                    break;
                //TODO if wavesize is 0, then this never changes. Maybe
                //set it directly instead of multiplying?
                case "w":
                    waveSize = baseWaveSize * modifier.value;
                    break;
            }
        }
    }
}