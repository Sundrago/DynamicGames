using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Wiggle", fileName = "Wiggle Behavior")]
    [EffectInfo("wiggle", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 4.74f)]
    [DefaultValue(nameof(baseFrequency), 7.82f)]
    [DefaultValue(nameof(baseWaveSize), .551f)]
    public sealed class WiggleBehavior : BehaviorScriptableSine
    {
        private const int maxDirections = 23;
        private Vector3[] directions;
        private int indexCache;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            directions = new Vector3[maxDirections];

            //Calculates a random direction for each character (which won't change)
            for (var i = 0; i < maxDirections; i++)
                directions[i] = TextUtilities.fakeRandoms[Random.Range(0, TextUtilities.fakeRandomsCount - 1)] *
                                Mathf.Sign(Mathf.Sin(i));
        }

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            indexCache = character.index % maxDirections;

            character.current.positions.MoveChar(
                directions[indexCache]
                * Mathf.Sin(animator.time.timeSinceStart * frequency + character.index * waveSize)
                * amplitude
                * character.uniformIntensity);
        }
    }
}