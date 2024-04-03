using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.System
{
    public class FX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems = new ParticleSystem[3];
        [SerializeField] private float durationInSec;

        private FXType fXType;

        public void OnParticleSystemStopped()
        {
            if (gameObject.activeSelf) FXManager.Instance.KillFX(this);
        }

        public void InitAndPlayFX(Vector3 target, FXType type)
        {
            fXType = type;
            gameObject.transform.position = target;

            foreach (var particle in particleSystems)
            {
                particle.time = 0;
                particle.Clear();
                particle.Play();
            }

            if (durationInSec != 0) Invoke("OnParticleSystemStopped", durationInSec);
        }

        public FXType GetFXType()
        {
            return fXType;
        }


#if UNITY_EDITOR
        [Button]
        private void AddParticlesFXs(FXType type)
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();

            var manager = gameObject.transform.GetComponentInParent<FXManager>();

            foreach (var fX in manager.FXDatas)
                if (fX.type == type)
                    return;

            var fxdata = new FXData();
            fxdata.prefab = this;
            fxdata.type = type;
            manager.FXDatas.Add(fxdata);

            gameObject.SetActive(false);
        }
#endif
    }
}