using System.Collections.Generic;
using UnityEngine;

namespace ETFXPEL
{
    public class ParticleEffectsLibrary : MonoBehaviour
    {
        public static ParticleEffectsLibrary GlobalAccess;

        // Stores total number of effects in arrays - NOTE: All Arrays must match length.
        public int TotalEffects;
        public int CurrentParticleEffectIndex;

        public int CurrentParticleEffectNum;

//	public string[] ParticleEffectDisplayNames;
        public Vector3[] ParticleEffectSpawnOffsets;

        // How long until Particle Effect is Destroyed - 0 = never
        public float[] ParticleEffectLifetimes;
        public GameObject[] ParticleEffectPrefabs;

        private void Awake()
        {
            GlobalAccess = this;

            currentActivePEList = new List<Transform>();

            TotalEffects = ParticleEffectPrefabs.Length;

            CurrentParticleEffectNum = 1;

            // Warn About Lengths of Arrays not matching
            if (ParticleEffectSpawnOffsets.Length != TotalEffects)
                Debug.LogError(
                    "ParticleEffectsLibrary-ParticleEffectSpawnOffset: Not all arrays match length, double check counts.");
            if (ParticleEffectPrefabs.Length != TotalEffects)
                Debug.LogError(
                    "ParticleEffectsLibrary-ParticleEffectPrefabs: Not all arrays match length, double check counts.");

            // Setup Starting PE Name String
            effectNameString = ParticleEffectPrefabs[CurrentParticleEffectIndex].name + " (" +
                               CurrentParticleEffectNum + " of " + TotalEffects + ")";
        }

        // Storing for deleting if looping particle effect
#pragma warning disable 414
        private string effectNameString = "";
#pragma warning disable 414
        private List<Transform> currentActivePEList;

        private void Start()
        {
        }

        public string GetCurrentPENameString()
        {
            return ParticleEffectPrefabs[CurrentParticleEffectIndex].name + " (" + CurrentParticleEffectNum + " of " +
                   TotalEffects + ")";
        }

        public void PreviousParticleEffect()
        {
            // Destroy Looping Particle Effects
            if (ParticleEffectLifetimes[CurrentParticleEffectIndex] == 0)
                if (currentActivePEList.Count > 0)
                {
                    for (var i = 0; i < currentActivePEList.Count; i++)
                        if (currentActivePEList[i] != null)
                            Destroy(currentActivePEList[i].gameObject);
                    currentActivePEList.Clear();
                }

            // Select Previous Particle Effect
            if (CurrentParticleEffectIndex > 0)
                CurrentParticleEffectIndex -= 1;
            else
                CurrentParticleEffectIndex = TotalEffects - 1;
            CurrentParticleEffectNum = CurrentParticleEffectIndex + 1;

            // Update PE Name String
            effectNameString = ParticleEffectPrefabs[CurrentParticleEffectIndex].name + " (" +
                               CurrentParticleEffectNum + " of " + TotalEffects + ")";
        }

        public void NextParticleEffect()
        {
            // Destroy Looping Particle Effects
            if (ParticleEffectLifetimes[CurrentParticleEffectIndex] == 0)
                if (currentActivePEList.Count > 0)
                {
                    for (var i = 0; i < currentActivePEList.Count; i++)
                        if (currentActivePEList[i] != null)
                            Destroy(currentActivePEList[i].gameObject);
                    currentActivePEList.Clear();
                }

            // Select Next Particle Effect
            if (CurrentParticleEffectIndex < TotalEffects - 1)
                CurrentParticleEffectIndex += 1;
            else
                CurrentParticleEffectIndex = 0;
            CurrentParticleEffectNum = CurrentParticleEffectIndex + 1;

            // Update PE Name String
            effectNameString = ParticleEffectPrefabs[CurrentParticleEffectIndex].name + " (" +
                               CurrentParticleEffectNum + " of " + TotalEffects + ")";
        }

        private Vector3 spawnPosition = Vector3.zero;

        public void SpawnParticleEffect(Vector3 positionInWorldToSpawn)
        {
            // Spawn Currently Selected Particle Effect
            spawnPosition = positionInWorldToSpawn + ParticleEffectSpawnOffsets[CurrentParticleEffectIndex];
            var newParticleEffect = Instantiate(ParticleEffectPrefabs[CurrentParticleEffectIndex], spawnPosition,
                ParticleEffectPrefabs[CurrentParticleEffectIndex].transform.rotation);
            newParticleEffect.name = "PE_" + ParticleEffectPrefabs[CurrentParticleEffectIndex];
            // Store Looping Particle Effects Systems
            if (ParticleEffectLifetimes[CurrentParticleEffectIndex] == 0)
                currentActivePEList.Add(newParticleEffect.transform);
            currentActivePEList.Add(newParticleEffect.transform);
            // Destroy Particle Effect After Lifetime expired
            if (ParticleEffectLifetimes[CurrentParticleEffectIndex] != 0)
                Destroy(newParticleEffect, ParticleEffectLifetimes[CurrentParticleEffectIndex]);
        }
    }
}