using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Build_BlockObj : MonoBehaviour
{
    [SerializeField]
    private Build_SFXManager sfxManager;
    void OnCollisionEnter2D(Collision2D collision) {
        ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
        collision.GetContacts(contacts);
        float totalImpulse = 0;
        foreach (ContactPoint2D contact in contacts) {
            totalImpulse += contact.normalImpulse;
        }
        StartCoroutine(sfxManager.PlaySFXByHitSize(totalImpulse));
        Debug.Log("totalImpulse : " + totalImpulse);
    }
}
