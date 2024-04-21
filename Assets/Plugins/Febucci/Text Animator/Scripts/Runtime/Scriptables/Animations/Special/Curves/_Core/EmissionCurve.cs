using System;
using UnityEngine;

namespace Febucci.UI.Effects
{
    public class EmissionCurveProperty : PropertyAttribute
    {
    }

    [Serializable]
    public class EmissionCurve
    {
        public int cycles;
        public float duration;
        [SerializeField] public AnimationCurve weightOverTime;

        public EmissionCurve()
        {
            cycles = -1;
            duration = 1;
            weightOverTime = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        }

        public EmissionCurve(params Keyframe[] keyframes)
        {
            cycles = -1;
            duration = 1;
            weightOverTime = new AnimationCurve(keyframes);
        }

        public float GetMaxDuration()
        {
            return cycles > 0 ? duration * cycles : -1;
        }

        public float Evaluate(float timePassed)
        {
            if (cycles > 0 && timePassed > duration * cycles) return 0;

            return weightOverTime.Evaluate(timePassed % duration);
        }
    }
}