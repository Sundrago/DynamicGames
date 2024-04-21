using System.Collections;
using UnityEngine;

namespace EpicToonFX
{
    public class ETFXLoopScript : MonoBehaviour
    {
        public GameObject chosenEffect;
        public float loopTimeLimit = 2.0f;

        [Header("Spawn options")] public bool disableLights = true;

        public bool disableSound = true;
        public float spawnScale = 1.0f;

        private void Start()
        {
            PlayEffect();
        }

        public void PlayEffect()
        {
            StartCoroutine("EffectLoop");
        }

        private IEnumerator EffectLoop()
        {
            var effectPlayer = Instantiate(chosenEffect, transform.position, transform.rotation);

            effectPlayer.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);

            if (disableLights && effectPlayer.GetComponent<Light>()) effectPlayer.GetComponent<Light>().enabled = false;

            if (disableSound && effectPlayer.GetComponent<AudioSource>())
                effectPlayer.GetComponent<AudioSource>().enabled = false;

            yield return new WaitForSeconds(loopTimeLimit);

            Destroy(effectPlayer);
            PlayEffect();
        }
    }
}