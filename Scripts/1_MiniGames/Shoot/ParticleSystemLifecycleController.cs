using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages the lifecycle of particle systems in a shooting mini-game.
    /// </summary>
    public class ParticleSystemLifecycleController : MonoBehaviour
    {
        [SerializeField] private BulletManager bulletManager;

        public void OnParticleSystemStopped()
        {
            bulletManager.KillParticleFX(gameObject.GetComponent<ParticleSystem>());
        }
    }
}