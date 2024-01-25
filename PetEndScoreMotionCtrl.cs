using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Febucci.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PetEndScoreMotionCtrl : MonoBehaviour
{
    [SerializeField] private SpriteAnimator spriteAnimator;
    [SerializeField] private Transform dialogueHolder;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private PetManager petManager;
    [SerializeField] private PetDialogueManager petDialogueManager;
    
    [Button]
    public void Init(PetType type, PetDialogueManager.PetScoreType scoreType)
    {
        Pet pet = petManager.GetPetDataByType(type).obj.GetComponent<Pet>();
        
        spriteAnimator.sprites = pet.GetRandomIdleAnim();
        typewriter.ShowText(petDialogueManager.GetPetScoreString(type, scoreType));
        typewriter.StartShowingText(true);

        spriteAnimator.GetComponent<SpriteRenderer>().sprite = spriteAnimator.sprites[0];
        spriteAnimator.gameObject.transform.localRotation = pet.spriteRenderer.transform.localRotation;
        spriteAnimator.gameObject.transform.localPosition = pet.spriteRenderer.transform.localPosition;
        spriteAnimator.gameObject.transform.localScale = pet.spriteRenderer.transform.localScale;

        spriteAnimator.interval = pet.GetInterval();

        dialogueHolder.localScale = Vector3.zero;
        dialogueHolder.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        gameObject.SetActive(true);
    }
}
