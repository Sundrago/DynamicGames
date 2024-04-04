using DG.Tweening;
using UnityEngine;

namespace DynamicGames.UI
{
    /// <summary>
    /// Responsible for animating particle emission in Unity.
    /// </summary>
    public class AnimateParticleEmission : MonoBehaviour
    {
        [SerializeField] private ParticleSystem systems;
        [SerializeField] private float rateOverTimeConstantMaxTo;
        [SerializeField] private float duration = 10f;

        private ParticleSystem.EmissionModule emission;

        private void Start()
        {
            emission = systems.emission;
            DOVirtual.Float(emission.rateOverTime.constantMax, rateOverTimeConstantMaxTo, duration,
                x => { emission.rateOverTime = new ParticleSystem.MinMaxCurve(0, x); });
        }
    }
}