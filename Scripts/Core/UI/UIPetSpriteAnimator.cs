using System.Collections;
using System.Collections.Generic;
using Core.Pet;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class UIPetSpriteAnimator : MonoBehaviour
{
    [SerializeField] private SpriteAnimator spriteAnimator;
    [SerializeField] private PetManager petManager;
    
    [Button]
    public void Init(PetType type)
    {
        Pet pet = petManager.GetPetDataByType(type).obj.GetComponent<Pet>();
        
        pet.SetSpriteAnimatorIdleAnimation(spriteAnimator);
        spriteAnimator.GetComponent<SpriteRenderer>().sprite = spriteAnimator.sprites[0];
        spriteAnimator.gameObject.transform.localRotation = pet.spriteRenderer.transform.localRotation;
        spriteAnimator.gameObject.transform.localPosition = pet.spriteRenderer.transform.localPosition;
        spriteAnimator.gameObject.transform.localScale = pet.spriteRenderer.transform.localScale;
        spriteAnimator.interval = pet.GetInterval();
    }
}
