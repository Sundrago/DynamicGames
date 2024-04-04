using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Manages the particle effects for the Land game.
    /// </summary>
    public class ParticleFXManager : MonoBehaviour
    {
        [Header("Game Components")] 
        [SerializeField] private Animator puffLeftAnimator, puffRightAnimator, puffFailAnimator;
        [SerializeField] private ParticleSystem thrustFX, thrustFX2, thrust_left, thrust_right;

        private ParticleSystem.EmissionModule thrustEmission, thrustEmission2, thrustLeftEmission, thrustRightEmission;

        public void InitializeParticleSystem()
        {
            thrustEmission = thrustFX.emission;
            thrustEmission2 = thrustFX2.emission;
            thrustLeftEmission = thrust_left.emission;
            thrustRightEmission = thrust_right.emission;
        }

        public void SetThrustLeftParticleEmission(bool isOn)
        {
            thrustRightEmission.rateOverTime = isOn ? 20 : 0;
        }

        public void SetThrustRightParticleEmission(bool isOn)
        {
            thrustLeftEmission.rateOverTime = isOn ? 20 : 0;
        }

        public void SetThrustParticleEmission(bool isOn)
        {
            thrustEmission.rateOverTime = isOn ? 60 : 0;
            thrustEmission2.rateOverTime = isOn ? 20 : 0;
            thrustLeftEmission.rateOverTime = 0;
            thrustRightEmission.rateOverTime = 0;
        }

        public void PlaySucceedFx(Transform rocket)
        {
            puffLeftAnimator.transform.position =
                new Vector2(rocket.position.x - 0.5f, puffLeftAnimator.transform.position.y);
            puffRightAnimator.transform.position =
                new Vector2(rocket.position.x + 0.5f, puffRightAnimator.transform.position.y);
            puffLeftAnimator.SetTrigger("puff_4");
            puffRightAnimator.SetTrigger("puff_4");
            SetThrustParticleEmission(false);
        }

        public void PlayFailedFx(Transform rocket, Vector3 failPosition)
        {
            puffFailAnimator.SetTrigger("puff_9");
            puffFailAnimator.transform.position = failPosition;
            puffFailAnimator.transform.rotation = rocket.transform.rotation;
            SetThrustParticleEmission(false);
        }
    }
}