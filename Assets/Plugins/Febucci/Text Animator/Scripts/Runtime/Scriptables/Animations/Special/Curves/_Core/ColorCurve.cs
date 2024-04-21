using System;
using UnityEngine;

namespace Febucci.UI.Effects
{
    public class ColorCurveProperty : PropertyAttribute
    {
    }

    [Serializable]
    public struct ColorCurve
    {
        public bool enabled;

        public Gradient colorOverTime;
        public float waveSize;
        public float duration;

        public ColorCurve(float waveSize)
        {
            enabled = false;
            this.waveSize = waveSize;
            duration = 1;
            colorOverTime = new Gradient();
            colorOverTime.SetKeys(
                new GradientColorKey[]
                {
                    new(Color.white, 0),
                    new(Color.cyan, 0.5f),
                    new(Color.white, 1)
                },
                new GradientAlphaKey[]
                {
                    new(1, 0),
                    new(1, 1)
                }
            );
        }

        public Color32 Evaluate(float time, int charIndex)
        {
            time = Mathf.Repeat(time + charIndex * waveSize, duration);
            return colorOverTime.Evaluate(time);
        }
    }
}