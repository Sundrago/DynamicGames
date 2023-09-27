using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseParticleSystem : MonoBehaviour
{
    public Shoot_Bullet_Manager manager;

    public void OnParticleSystemStopped()
    {
        manager.KillParticleFX(gameObject.GetComponent<ParticleSystem>());
    }
}
