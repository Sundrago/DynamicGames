using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;
using DG.Tweening;

public class EnergyUIFXCtrl : MonoBehaviour
{
    [SerializeField] GameObject targetGameObj;
    [SerializeField] ParticleImage shine;
    [SerializeField] ParticleImage ring;
    
    bool isActve = false;

    void Update()
    {
        if(!isActve) return;
        if(targetGameObj != null) gameObject.transform.position = targetGameObj.transform.position;
    }

    public void InitiateFX(GameObject obj = null) {
        targetGameObj = obj;

        isActve = true;
        shine.rateOverTime = 8;
        ring.rateOverTime = 1;
        gameObject.SetActive(true);
    }

    public void DestroyFX()
    {
        if (!isActve) return;
        isActve = false;
        shine.rateOverTime = 0;
        ring.rateOverTime = 0;
        DOVirtual.DelayedCall(5f, () => {
            if(gameObject != null)
            Destroy(gameObject);
        });
    }
}
