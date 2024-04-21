using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EpicToonFX
{
    public class ETFXEffectControllerPooled : MonoBehaviour
    {
        public GameObject[] effects;

        [Space(10)] [Header("Spawn Settings")] public bool disableLights = true;

        public bool disableSound = true;
        public float startDelay = 0.2f;
        public float respawnDelay = 0.5f;
        public bool slideshowMode;
        public bool autoRotation;

        [Range(0.001f, 0.5f)] public float autoRotationSpeed = 0.1f;

        private GameObject currentEffect;
        private int effectIndex;
        private Text effectIndexText;
        private Text effectNameText;
        private List<GameObject> effectsPool;

        private ETFXMouseOrbit etfxMouseOrbit;

        //Caching components
        private void Awake()
        {
            effectNameText = GameObject.Find("EffectName").GetComponent<Text>();
            effectIndexText = GameObject.Find("EffectIndex").GetComponent<Text>();

            etfxMouseOrbit = Camera.main.GetComponent<ETFXMouseOrbit>();
            etfxMouseOrbit.etfxEffectControllerPooled = this;

            //Pooling
            effectsPool = new List<GameObject>();

            for (var i = 0; i < effects.Length; i++)
            {
                var effect = Instantiate(effects[i], transform.position, Quaternion.identity);
                effect.transform.parent = transform;
                effectsPool.Add(effect);

                effect.SetActive(false);
            }
        }

        private void Start()
        {
            Invoke("InitializeLoop", startDelay);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) NextEffect();

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PreviousEffect();
        }

        private void FixedUpdate()
        {
            if (autoRotation)
            {
                etfxMouseOrbit.SetAutoRotationSpeed(autoRotationSpeed);

                if (!etfxMouseOrbit.isAutoRotating)
                    etfxMouseOrbit.InitializeAutoRotation();
            }
        }

        public void InitializeLoop()
        {
            StartCoroutine(EffectLoop());
        }

        public void NextEffect()
        {
            if (effectIndex < effects.Length - 1)
                effectIndex++;
            else
                effectIndex = 0;

            CleanCurrentEffect();
        }

        public void PreviousEffect()
        {
            if (effectIndex > 0)
                effectIndex--;
            else
                effectIndex = effects.Length - 1;

            CleanCurrentEffect();
        }

        private void CleanCurrentEffect()
        {
            StopAllCoroutines();

            if (currentEffect != null) currentEffect.SetActive(false);

            StartCoroutine(EffectLoop());
        }

        private IEnumerator EffectLoop()
        {
            //Pooling effect
            currentEffect = effectsPool[effectIndex];
            currentEffect.SetActive(true);

            if (disableLights && currentEffect.GetComponent<Light>())
                currentEffect.GetComponent<Light>().enabled = false;

            if (disableSound && currentEffect.GetComponent<AudioSource>())
                currentEffect.GetComponent<AudioSource>().enabled = false;

            //Update UI
            effectNameText.text = effects[effectIndex].name;
            effectIndexText.text = effectIndex + 1 + " of " + effects.Length;

            var particleSystem = currentEffect.GetComponent<ParticleSystem>();

            while (true)
            {
                yield return new WaitForSeconds(particleSystem.main.duration + respawnDelay);

                if (!slideshowMode)
                {
                    if (!particleSystem.main.loop)
                    {
                        currentEffect.SetActive(false);
                        currentEffect.SetActive(true);
                    }
                }
                else
                {
                    //Double delay for looping effects
                    if (particleSystem.main.loop) yield return new WaitForSeconds(respawnDelay);

                    NextEffect();
                }
            }
        }
    }
}