using System;
using UnityEngine;

namespace Febucci.UI.Effects
{
    public class FloatCurveProperty : PropertyAttribute
    {
    }

    [Serializable] //TODO test
    public struct FloatCurve
    {
        public bool enabled;
        public AnimationCurve weightOverTime;
        public float amplitude;
        public float waveSize;

        private readonly float defaultAmplitude;

        public FloatCurve(float amplitude, float waveSize, float defaultAmplitude)
        {
            this.defaultAmplitude = defaultAmplitude;
            enabled = false;
            this.amplitude = amplitude;
            weightOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, .5f), new Keyframe(1, 0));
            weightOverTime.preWrapMode = WrapMode.Loop;
            weightOverTime.postWrapMode = WrapMode.Loop;
            this.waveSize = 0;
        }

        public float Evaluate(float passedTime, int charIndex)
        {
            if (!enabled) return defaultAmplitude;

            return Mathf.LerpUnclamped(defaultAmplitude, amplitude,
                weightOverTime.Evaluate(passedTime + waveSize * charIndex));
        }
    }
}