using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core.Pet
{
    public class NewPetAnimManager : MonoBehaviour
    {
        private const float Anim2DelaySeconds = 5.2f;
        private const float CompleteDelaySeconds = 7f;
        
        [Header("Managers and Controllers")] 
        [SerializeField] private SfxController soundFxController;
        [SerializeField] private NewPetAnim_IntroSequence petAnim1IntroSequence;
        [SerializeField] private NewPetAnim_PreviewSequence petAnim2PreviewSequence;
        [SerializeField] private SpriteAnimator petSpriteAnimator; 
        
        [Header("Game Components")] 
        [SerializeField] private Volume postProcessingGlowVolume;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Image backgroundImage;

        private bool isAnimationComplete;
        private PetType selectedPetType;
        
        [Sirenix.OdinInspector.Button]
        public void Init(PetType petType)
        {
            SetInitialSettings(petType);
            
            InitiateAnimation1();
            InitiateAnimation2();
        }

        private Sprite[] GetPetWalkAnim()
        {
            var petData = PetManager.Instance.GetPetDataByType(selectedPetType).obj;
            return petData.GetComponent<Pet>().GetWalkAnim();
        }
        
        private void SetInitialSettings(PetType petType)
        {
            soundFxController.PauseBGM();
            selectedPetType = petType;
            gameObject.SetActive(true);
            gameObject.transform.localPosition = Vector3.zero;
            isAnimationComplete = false;

            audioSource.Play();
            postProcessingGlowVolume.weight = 0;
            petAnim1IntroSequence.gameObject.SetActive(false);
            petAnim2PreviewSequence.gameObject.SetActive(false);
            backgroundImage.color = Color.white;
            
            backgroundImage.DOColor(Color.black, 2f);
            petSpriteAnimator.sprites = GetPetWalkAnim();
        }

        private void InitiateAnimation1()
        {
            petAnim1IntroSequence.Init(PetDialogueManager.Instance.GetWelcomeString(selectedPetType));
            DOVirtual.Float(0f, 1f, 2f, (x) => { postProcessingGlowVolume.weight = x; });
        }

        private void InitiateAnimation2()
        {
            DOVirtual.DelayedCall(Anim2DelaySeconds,
                () =>
                {
                    petAnim2PreviewSequence.Init(selectedPetType.ToString().ToUpper(), PetDialogueManager.Instance.GetDescrString(selectedPetType),
                        PetDialogueManager.Instance.GetRank(selectedPetType).ToString());
                });
            DOVirtual.Float(1f, 0.55f, 1f, (x) => { postProcessingGlowVolume.weight = x; }).SetDelay(Anim2DelaySeconds);

            DOVirtual.DelayedCall(CompleteDelaySeconds, () =>
            {
                soundFxController.UnPauseBGM();
                isAnimationComplete = true;
            });
        }

        public void Close()
        {
            if (!isAnimationComplete) return;

            gameObject.transform.DOMoveY(-3000, 1f).SetEase(Ease.InOutExpo)
                .OnComplete(() => { gameObject.SetActive(false); });
            DOVirtual.Float(0.55f, 0, 0.6f, (x) => { postProcessingGlowVolume.weight = x; });
        }
    }
}