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
        PetController petController = petManager.GetPetDataByType(type).obj.GetComponent<PetController>();
        
        petController.SetSpriteAnimatorIdleAnimation(spriteAnimator);
        spriteAnimator.GetComponent<SpriteRenderer>().sprite = spriteAnimator.sprites[0];
        spriteAnimator.gameObject.transform.localRotation = petController.spriteRenderer.transform.localRotation;
        spriteAnimator.gameObject.transform.localPosition = petController.spriteRenderer.transform.localPosition;
        spriteAnimator.gameObject.transform.localScale = petController.spriteRenderer.transform.localScale;
        spriteAnimator.interval = petController.GetInterval();
    }
}
