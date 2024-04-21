using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(fileName = "Horizontal Expand Appearance",
        menuName = "Text Animator/Animations/Appearances/Horizontal Expand")]
    [EffectInfo("horiexp", EffectCategory.Appearances)]
    public sealed class HorizontalExpandAppearance : AppearanceScriptableBase
    {
        //expand type
        public enum ExpType
        {
            Left, //from left to right
            Middle, //expands from the middle to te extents
            Right //from right to left
        }


        public ExpType type = ExpType.Left;
        private float pct;
        private Vector2 startBot;

        //--Temp variables--
        private Vector2 startTop;


        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);
            type = ExpType.Left;
        }


        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            pct = Tween.EaseInOut(character.passedTime / duration);

            switch (type)
            {
                default:
                case ExpType.Left:
                    //top left and bot left
                    startTop = character.current.positions[1];
                    startBot = character.current.positions[0];

                    character.current.positions[2] =
                        Vector3.LerpUnclamped(startTop, character.current.positions[2], pct);
                    character.current.positions[3] =
                        Vector3.LerpUnclamped(startBot, character.current.positions[3], pct);
                    break;

                case ExpType.Right:
                    //top right and bot right
                    startTop = character.current.positions[2];
                    startBot = character.current.positions[3];

                    character.current.positions[1] =
                        Vector3.LerpUnclamped(startTop, character.current.positions[1], pct);
                    character.current.positions[0] =
                        Vector3.LerpUnclamped(startBot, character.current.positions[0], pct);
                    break;

                case ExpType.Middle:
                    //Middle positions
                    startTop = (character.current.positions[1] + character.current.positions[2]) / 2;
                    startBot = (character.current.positions[0] + character.current.positions[3]) / 2;

                    //top vertices
                    character.current.positions[1] =
                        Vector3.LerpUnclamped(startTop, character.current.positions[1], pct);
                    character.current.positions[2] =
                        Vector3.LerpUnclamped(startTop, character.current.positions[2], pct);

                    //bottom vertices
                    character.current.positions[0] =
                        Vector3.LerpUnclamped(startBot, character.current.positions[0], pct);
                    character.current.positions[3] =
                        Vector3.LerpUnclamped(startBot, character.current.positions[3], pct);

                    break;
            }
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "x":
                    switch (modifier.value)
                    {
                        case -1:
                            type = ExpType.Left;
                            break;
                        case 0:
                            type = ExpType.Middle;
                            break;
                        case 1:
                            type = ExpType.Right;
                            break;
                        default:
                            Debug.LogError(
                                $"Text Animator: you set an '{modifier.name}' modifier with value '{modifier.value}' for the HorizontalExpandAppearance effect, but it can only be '-1', '0', or '1'");
                            break;
                    }

                    break;
                default:
                    base.SetModifier(modifier);
                    break;
            }
        }
    }
}