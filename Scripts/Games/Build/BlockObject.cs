using System.Collections.Generic;
using UnityEngine;

namespace Games.Build
{
    /// <summary>
    ///     Class responsible for representing a block object in a build.
    /// </summary>
    public class BlockObject : MonoBehaviour
    {
        [SerializeField] private SFXManager sfxManager;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var totalImpulse = CalculateTotalImpulse(collision.contacts);
            sfxManager?.PlaySFXByHitSize(totalImpulse);
            Debug.Log($"Total impulse: {totalImpulse}");
        }

        private float CalculateTotalImpulse(IEnumerable<ContactPoint2D> contacts)
        {
            var totalImpulse = 0f;
            foreach (var contact in contacts) totalImpulse += contact.normalImpulse;

            return totalImpulse;
        }
    }
}