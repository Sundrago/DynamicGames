using System.Collections;
using UnityEngine;

namespace EpicToonFX
{
    public class ETFXTarget : MonoBehaviour
    {
        [Header("Effect shown on target hit")] public GameObject hitParticle;

        [Header("Effect shown on target respawn")]
        public GameObject respawnParticle;

        private Collider targetCollider;
        private Renderer targetRenderer;

        private void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            targetCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Missile") // If collider is tagged as missile
                if (hitParticle)
                {
                    //Debug.Log("Target hit!");
                    var destructibleEffect =
                        Instantiate(hitParticle, transform.position, transform.rotation); // Spawns attached hit effect
                    Destroy(destructibleEffect, 2f); // Removes hit effect after x seconds
                    targetRenderer.enabled = false; // Hides the target
                    targetCollider.enabled = false; // Disables target collider
                    StartCoroutine(Respawn()); // Sets timer for respawning the target
                }
        }

        private void SpawnTarget()
        {
            targetRenderer.enabled = true; //Shows the target
            targetCollider.enabled = true; //Enables the collider
            var respawnEffect =
                Instantiate(respawnParticle, transform.position, transform.rotation); //Spawns attached respawn effect
            Destroy(respawnEffect, 3.5f); //Removes attached respawn effect after x seconds
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(3);
            SpawnTarget();
        }
    }
}