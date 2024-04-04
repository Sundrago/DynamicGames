using DG.Tweening;
using UnityEngine;

namespace Core.UI
{
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