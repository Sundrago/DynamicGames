using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(fileName = "Uniform Curve Animation", menuName = "Text Animator/Animations/Special/Uniform Curve")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class UniformCurveAnimation : AnimationScriptableBase
    {
        public TimeMode timeMode = new(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new();
        public AnimationData animationData = new();

        private bool hasTransformEffects;

        private float timePassed;
        private float timeSpeed;

        //--- Modifiers ---
        private float weightMult;


        public override void ResetContext(TAnimCore animator)
        {
            weightMult = 1;
            timeSpeed = 1;
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "f": //frequency, increases the time speed
                    timeSpeed = modifier.value;
                    break;

                case "a": //increase the amplitude
                    weightMult = modifier.value;
                    break;
            }
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            timePassed = timeMode.GetTime(animator.time.timeSinceStart * timeSpeed, character.passedTime * timeSpeed,
                character.index);
            if (timePassed < 0)
                return;

            var weight = weightMult * emissionCurve.Evaluate(timePassed);

            if (animationData.TryCalculatingMatrix(character, timePassed, weight, out var matrix, out var offset))
                for (byte i = 0; i < TextUtilities.verticesPerChar; i++)
                    character.current.positions[i] =
                        matrix.MultiplyPoint3x4(character.current.positions[i] - offset) + offset;

            if (animationData.TryCalculatingColor(character, timePassed, weight, out var color))
                character.current.colors.LerpUnclamped(color, Mathf.Clamp01(weight));
        }

        public override float GetMaxDuration()
        {
            return emissionCurve.GetMaxDuration();
        }

        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator)
        {
            return true;
        }
    }
}