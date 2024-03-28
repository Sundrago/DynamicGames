using UnityEngine;

namespace Games.Shoot
{
    public class ParticleSystemLifecycleController : MonoBehaviour
    {
        [SerializeField] private BulletManager bulletManager;

        public void OnParticleSystemStopped()
        {
            bulletManager.KillParticleFX(gameObject.GetComponent<ParticleSystem>());
        }
    }
}