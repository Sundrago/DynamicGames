using Febucci.Attributes;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace Febucci.UI
{
    /// <summary>
    ///     Built-in typewriter, which shows letters dynamically word after word.<br />
    ///     To enable it, add this component near a <see cref="Febucci.UI.Core.TAnimCore" /> one<br />
    ///     - Base class: <see cref="Febucci.UI.Core.TypewriterCore" /><br />
    ///     - Manual: <see href="https://www.febucci.com/text-animator-unity/docs/typewriters/">TextAnimatorPlayers</see>
    /// </summary>
    [HelpURL("https://www.febucci.com/text-animator-unity/docs/typewriters/")]
    [AddComponentMenu("Febucci/TextAnimator/Typewriter - By Word")]
    public class TypewriterByWord : TypewriterCore
    {
        [SerializeField] [CharsDisplayTime] public float waitForNormalWord = 0.3f;

        [FormerlySerializedAs("waitForWordWithPuntuaction")] [SerializeField] [CharsDisplayTime]
        public float waitForWordWithPunctuation = 0.5f;

        [SerializeField] [CharsDisplayTime] public float disappearanceDelay = 0.5f;

        private bool IsCharInsideAnyWord(int charIndex)
        {
            return TextAnimator.Characters[charIndex].wordIndex >= 0;
        }

        protected override float GetWaitAppearanceTimeOf(int charIndex)
        {
            if (!IsCharInsideAnyWord(charIndex) && TextAnimator.latestCharacterShown.index > 0)
            {
                var latestWordShownIndex =
                    TextAnimator.Characters[TextAnimator.latestCharacterShown.index - 1].wordIndex;
                if (latestWordShownIndex >= 0 && latestWordShownIndex < TextAnimator.WordsCount)
                {
                    var word = TextAnimator.Words[latestWordShownIndex];
                    return char.IsPunctuation(TextAnimator.Characters[word.lastCharacterIndex].info.character)
                        ? waitForWordWithPunctuation
                        : waitForNormalWord;
                }

                return waitForNormalWord;
            }

            return 0;
        }

        protected override float GetWaitDisappearanceTimeOf(int charIndex)
        {
            return !IsCharInsideAnyWord(charIndex) ? disappearanceDelay : 0;
        }
    }
}