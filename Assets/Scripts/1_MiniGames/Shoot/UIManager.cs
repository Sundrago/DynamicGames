using DynamicGames.UI;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class UIManager
    {
        [SerializeField] private TutorialAnimation tutorialAnimation;
        [SerializeField] public ItemInformationUI itemInformationUIAtk;
        [SerializeField] public ItemInformationUI itemInformationUIShield;
        [SerializeField] public ItemInformationUI itemInformationUIBounce;
        [SerializeField] public ItemInformationUI itemInformationUISpin;
        [SerializeField] private GameObject tutorial;

        public UIManager()
        {
        }

        public TutorialAnimation TutorialAnimation
        {
            set { tutorialAnimation = value; }
            get { return tutorialAnimation; }
        }

        public GameObject Tutorial
        {
            set { tutorial = value; }
            get { return tutorial; }
        }

        public void SetFaceAnimation(FaceState state, GameManager gameManager)
        {
            switch (state)
            {
                case FaceState.Idle:
                    gameManager.FaceAnimator.SetTrigger("idle");
                    break;
                case FaceState.TurnRed:
                    gameManager.FaceAnimator.SetTrigger("turnRed");
                    break;
                case FaceState.Angry01:
                    gameManager.FaceAnimator.SetTrigger("angry01");
                    break;
            }
        }
    }
}