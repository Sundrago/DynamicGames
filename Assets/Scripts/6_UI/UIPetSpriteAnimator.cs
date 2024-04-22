using DynamicGames.Pet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.UI
{
    /// <summary>
    /// Responsible for initializing the SpriteAnimator component of a pet object.
    /// </summary>
    public class UIPetSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteAnimator spriteAnimator;
        [SerializeField] private PetManager petManager;

        [Button]
        public void Init(PetType type)
        {
            var petObject = petManager.GetPetDataByType(type).obj.GetComponent<PetObject>();

            petObject.SetSpriteAnimatorIdleAnimation(spriteAnimator);
            spriteAnimator.GetComponent<SpriteRenderer>().sprite = spriteAnimator.sprites[0];
            spriteAnimator.gameObject.transform.localRotation = petObject.spriteRenderer.transform.localRotation;
            spriteAnimator.gameObject.transform.localPosition = petObject.spriteRenderer.transform.localPosition;
            spriteAnimator.gameObject.transform.localScale = petObject.spriteRenderer.transform.localScale;
            spriteAnimator.interval = petObject.GetInterval();
        }
    }
}