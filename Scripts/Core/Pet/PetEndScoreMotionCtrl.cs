using Core.UI;
using DG.Tweening;
using Febucci.UI;
using UnityEngine;

namespace Core.Pet
{
    public class PetEndScoreMotionCtrl : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private PetManager petManager;
        [SerializeField] private PetDialogueManager petDialogueManager;

        [Header("UI Components")] 
        [SerializeField] private SpriteAnimator spriteAnimator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform dialogueHolder;
        [SerializeField] private TypewriterByCharacter typewriter;


        public void Init(PetType type, PetScoreType scoreType)
        {
            var petController = petManager.GetPetDataByType(type).obj.GetComponent<PetObject>();

            petController.SetSpriteAnimatorIdleAnimation(spriteAnimator);

            spriteRenderer.sprite = spriteAnimator.sprites[0];
            spriteAnimator.gameObject.transform.localRotation = petController.spriteRenderer.transform.localRotation;
            spriteAnimator.gameObject.transform.localPosition = petController.spriteRenderer.transform.localPosition;
            spriteAnimator.gameObject.transform.localScale = petController.spriteRenderer.transform.localScale;

            spriteAnimator.interval = petController.GetInterval();

            dialogueHolder.localScale = Vector3.zero;
            dialogueHolder.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            gameObject.SetActive(true);
            typewriter.ShowText(petDialogueManager.GetPetScoreString(type, scoreType));
            typewriter.StartShowingText(true);
        }
    }
}