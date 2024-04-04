using Core.System;
using Core.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Core.Pet
{
    public class NewPetAnimManager : MonoBehaviour
    {
        private const float Anim2DelaySeconds = 5.2f;
        private const float CompleteDelaySeconds = 7f;

        [Header("Managers and Controllers")] [SerializeField]
        private SfxController soundFxController;

        [SerializeField] private NewPetAnimIntroSequence newPetAnimIntroSequence;
        [SerializeField] private NewPetAnimPreviewSequence newPetAnimPreviewSequence;
        [SerializeField] private SpriteAnimator petSpriteAnimator;

        [Header("Game Components")] 
        [SerializeField] private Volume postProcessingGlowVolume;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Image backgroundImage;

        private bool isAnimationComplete;
        private PetType selectedPetType;

        [Button]
        public void Init(PetType petType)
        {
            SetInitialSettings(petType);

            InitiateAnimation1();
            InitiateAnimation2();
        }

        private Sprite[] GetPetWalkAnim()
        {
            var petData = PetManager.Instance.GetPetDataByType(selectedPetType).obj;
            return petData.GetComponent<PetObject>().GetWalkAnim();
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
            newPetAnimIntroSequence.gameObject.SetActive(false);
            newPetAnimPreviewSequence.gameObject.SetActive(false);
            backgroundImage.color = Color.white;

            backgroundImage.DOColor(Color.black, 2f);
            petSpriteAnimator.sprites = GetPetWalkAnim();
        }

        private void InitiateAnimation1()
        {
            newPetAnimIntroSequence.Init(PetDialogueManager.Instance.GetWelcomeString(selectedPetType));
            DOVirtual.Float(0f, 1f, 2f, x => { postProcessingGlowVolume.weight = x; });
        }

        private void InitiateAnimation2()
        {
            DOVirtual.DelayedCall(Anim2DelaySeconds,
                () =>
                {
                    newPetAnimPreviewSequence.Init(selectedPetType.ToString().ToUpper(),
                        PetDialogueManager.Instance.GetDescrString(selectedPetType),
                        PetDialogueManager.Instance.GetRank(selectedPetType).ToString());
                });
            DOVirtual.Float(1f, 0.55f, 1f, x => { postProcessingGlowVolume.weight = x; }).SetDelay(Anim2DelaySeconds);

            DOVirtual.DelayedCall(CompleteDelaySeconds, () =>
            {
                soundFxController.ResumeBGM();
                isAnimationComplete = true;
            });
        }

        public void Close()
        {
            if (!isAnimationComplete) return;

            gameObject.transform.DOMoveY(-3000, 1f).SetEase(Ease.InOutExpo)
                .OnComplete(() => { gameObject.SetActive(false); });
            DOVirtual.Float(0.55f, 0, 0.6f, x => { postProcessingGlowVolume.weight = x; });
        }
    }
}