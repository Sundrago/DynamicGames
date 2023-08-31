using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
            particle.Clear();
            particle.Play();
        }

        if(fXType == FXType.Bomb)
        {
            GetComponent<Shoot_FX>().KillEnemyIfInDistance();
        }

        if(durationInSec != 0) Invoke("OnParticleSystemStopped", durationInSec);
    }

    public void OnParticleSystemStopped()
    {
        if (gameObject.activeSelf) FXManager.Instance.KillFX(this);
    }

    public FXType GetFXType()
    {
        return fXType;
    }


    [Button]
    private void AutoAddParticles(FXType type)
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        FXManager manager = gameObject.transform.GetComponentInParent<FXManager>();
        FXData fxdata = new FXData();
        fxdata.prefab = this;
        fxdata.type = type;
        manager.FXDatas.Add(fxdata);
    }
}
