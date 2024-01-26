using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimateParticleEmission : MonoBehaviour
{
    [SerializeField] private ParticleSystem systems;
    [SerializeField] private float rateOverTimeConstantMaxTo = 0;
    [SerializeField] private float duration = 10f;

    private ParticleSystem.EmissionModule emission;
    private void Start()
    {
        emission = systems.emission;
        DOVirtual.Float(emission.rateOverTime.constantMax, rateOverTimeConstantMaxTo, duration, (x) =>
        {
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(0, x);
        });
    }
}
