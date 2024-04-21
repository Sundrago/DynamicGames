using UnityEngine;

namespace EpicToonFX
{
    public class ETFXPitchRandomizer : MonoBehaviour
    {
        public float randomPercent = 10;

        private void Start()
        {
            transform.GetComponent<AudioSource>().pitch *= 1 + Random.Range(-randomPercent / 100, randomPercent / 100);
        }
    }
}