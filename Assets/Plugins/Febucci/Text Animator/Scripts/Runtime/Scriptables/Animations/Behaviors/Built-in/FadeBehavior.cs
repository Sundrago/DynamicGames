using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(fileName = "Fade Behavior", menuName = "Text Animator/Animations/Behaviors/Fade")]
    [EffectInfo("fade", EffectCategory.Behaviors)]
    public sealed class FadeBehavior : BehaviorScriptableBase
    {
        public float baseSpeed = .5f;
        public float baseDelay = 1f;
        private float delay;
        private Color32 temp;
        private float timeToShow;

        public override void ResetContext(TAnimCore animator)
        {
            delay = baseDelay;
            SetTimeToShow(baseSpeed);
        }

        //given speed (per second), sets the time needed to show
        private void SetTimeToShow(float speed)
        {
            timeToShow = 1 / speed;
            //TODO check for zero
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "f":
                    SetTimeToShow(baseSpeed * modifier.value);
                    break;
                case "d":
                    delay = baseDelay * modifier.value;
                    break;
            }
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            if (character.passedTime <= delay) //not passed enough time yet
                return;

            var charPct = (character.passedTime - delay) / timeToShow;

            if (charPct > 1) charPct = 1;

            //Lerps
            if (charPct < 1 && charPct >= 0)
                for (var i = 0; i < TextUtilities.verticesPerChar; i++)
                {
                    temp = character.current.colors[i];
                    temp.a = 0;

                    character.current.colors[i] =
                        Color32.LerpUnclamped(character.current.colors[i], temp, Tween.EaseInOut(charPct));
                }
            else //Keeps them hidden
                for (var i = 0; i < TextUtilities.verticesPerChar; i++)
                {
                    temp = character.current.colors[i];
                    temp.a = 0;

                    character.current.colors[i] = temp;
                }
        }
    }
}