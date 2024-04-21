using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Febucci.UI.Effects
{
    [Preserve]
    [CreateAssetMenu(fileName = "Vertex Curve Animation",
        menuName = "Text Animator/Animations/Special/Vertex Curve Animation")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class VertexCurveAnimation : AnimationScriptableBase
    {
        public TimeMode timeMode = new(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new();

        [SerializeField]
        private AnimationData[] animationPerVertexData = new AnimationData[TextUtilities.verticesPerChar];

        private Color32 color;

        //--- Management ---
        private Matrix4x4 matrix;
        private Vector3 movement;
        private Vector3 offset;
        private Quaternion rot;
        private Vector2 scale;

        private float timePassed;

        //--- Modifiers ---
        private float timeSpeed;
        private float weightMult;

        public AnimationData[] VertexData
        {
            get => animationPerVertexData;
            set
            {
                animationPerVertexData = value;
                ClampVertexDataArray();
            }
        }

        private void OnValidate()
        {
            ClampVertexDataArray();
        }

        public override void ResetContext(TAnimCore animator)
        {
            weightMult = 1;
            timeSpeed = 1;
            ClampVertexDataArray();
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

            for (byte i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                if (animationPerVertexData[i]
                    .TryCalculatingMatrix(character, timePassed, weight, out matrix, out offset))
                    character.current.positions[i] =
                        matrix.MultiplyPoint3x4(character.current.positions[i] - offset) + offset;

                if (animationPerVertexData[i].TryCalculatingColor(character, timePassed, weight, out color))
                    character.current.colors[i] =
                        Color32.LerpUnclamped(character.current.colors[i], color, Mathf.Clamp01(weight));
            }
        }

        public override float GetMaxDuration()
        {
            return emissionCurve.GetMaxDuration();
        }

        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator)
        {
            return true;
        }

        private void ClampVertexDataArray()
        {
            for (var i = 0; i < animationPerVertexData.Length; i++)
                if (animationPerVertexData[i] == null)
                    animationPerVertexData[i] = new AnimationData();

            if (animationPerVertexData.Length != TextUtilities.verticesPerChar)
            {
                Debug.LogError("Vertex data array must have four vertices. Clamping/Resizing to four.");

                var newArray = new AnimationData[TextUtilities.verticesPerChar];
                for (var i = 0; i < newArray.Length; i++)
                    if (i < animationPerVertexData.Length)
                        newArray[i] = animationPerVertexData[i];
                    else
                        newArray[i] = new AnimationData();
                animationPerVertexData = newArray;
            }
        }
    }
}