using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Shake", fileName = "Shake Behavior")]
    [EffectInfo("shake", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 1.13f)]
    [DefaultValue(nameof(baseDelay), .1f)]
    [DefaultValue(nameof(baseWaveSize), .45f)]
    public sealed class ShakeBehavior : BehaviorScriptableBase
    {
        //---BASE VALUES---
        public float baseAmplitude = .085f;
        public float baseDelay = .04f;
        public float baseWaveSize = .2f;

        private float amplitude;
        private float delay;

        private int randIndex;
        private float timePassed;
        private float waveSize;

        private void OnValidate()
        {
            ClampValues();
        }

        //--- ANIMATION ---

        private void ClampValues()
        {
            delay = Mathf.Clamp(delay, 0.002f, 500);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            randIndex = Random.Range(0, TextUtilities.fakeRandomsCount);
        }

        public override void ResetContext(TAnimCore animator)
        {
            amplitude = baseAmplitude;
            delay = baseDelay;
            waveSize = baseWaveSize;

            ClampValues();
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a":
                    amplitude = baseAmplitude * modifier.value;
                    break;
                case "d":
                    delay = baseDelay * modifier.value;
                    break;
                case "w":
                    waveSize = baseWaveSize * modifier.value;
                    break;
            }

            ClampValues();
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            timePassed = animator.time.timeSinceStart;
            timePassed += character.index * waveSize;

            randIndex = Mathf.RoundToInt(timePassed / delay) % TextUtilities.fakeRandomsCount;
            if (randIndex < 0) randIndex *= -1; //always positive

            character.current.positions.MoveChar
            (
                TextUtilities.fakeRandoms[randIndex] * amplitude * character.uniformIntensity
            );
        }
    }
}