using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for representing a block object in a build.
/// </summary>
public class Build_BlockObj : MonoBehaviour
{
    [SerializeField] private Build_SFXManager sfxManager;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float totalImpulse = CalculateTotalImpulse(collision.contacts);
        sfxManager?.PlaySFXByHitSize(totalImpulse); 
        Debug.Log($"Total impulse: {totalImpulse}");
    }

    private float CalculateTotalImpulse(IEnumerable<ContactPoint2D> contacts)
    {
        float totalImpulse = 0f;
        foreach (var contact in contacts)
        {
            totalImpulse += contact.normalImpulse;
        }
        return totalImpulse;
    }
}
