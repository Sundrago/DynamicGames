using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine;

namespace DynamicGames.MainPage
{
    /// <summary>
    ///     Controls the shine and ring particle effects.
    /// </summary>
    public class ShineFxController : MonoBehaviour
    {
        [SerializeField] private GameObject targetGameObj;
        [SerializeField] private ParticleImage shine;
        [SerializeField] private ParticleImage ring;

        private bool isActve;

        private void Update()
        {
            if (!isActve) return;
            if (targetGameObj != null) gameObject.transform.position = targetGameObj.transform.position;
        }

        public void InitiateFX(GameObject obj = null)
        {
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
            DOVirtual.DelayedCall(5f, () =>
            {
                if (gameObject != null)
                    Destroy(gameObject);
            });
        }
    }
}