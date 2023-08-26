using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour
{
    [SerializeField] ParticleSystem[] particleSystems = new ParticleSystem[3];
    [SerializeField] float durationInSec;

    private FXType fXType;

    public void InitAndPlayFX(Vector3 target, FXType type)
    {
        fXType = type;
        gameObject.transform.position = target;

        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Play();
        }

        if(fXType == FXType.Bomb)
        {
            GetComponent<Shoot_FX>().KillEnemyIfInDistance();
        }

        Invoke("OnParticleSystemStopped", durationInSec);
    }

    public void OnParticleSystemStopped()
    {
        if (gameObject.activeSelf) FXManager.Instance.KillFX(this);
    }

    public FXType GetFXType()
    {
        return fXType;
    }

    
}
