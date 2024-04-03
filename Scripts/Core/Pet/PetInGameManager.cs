using Core.System;
using Games.Shoot;
using UnityEngine;

namespace Core.Pet
{
    public class PetInGameManager : MonoBehaviour
    {
        public static PetInGameManager Instance { get; private set; }

        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager shoot;
        [SerializeField] private Games.Jump.GameManager jump;
        [SerializeField] private Games.Land.GameManager land;
        [SerializeField] private Games.Build.GameManager build;
        [SerializeField] public PetController petController;
        
        private BlockStatusManager.BlockStatusData blockData;
        private GameType gameType;
        private float selectedTime;
        private bool enterGameWithPet;
        public bool EnterGameWithPet => enterGameWithPet;

        private void Awake()
        {
            Instance = this;
        }

        public void PetSelected(BlockStatusManager.BlockStatusData blockData, PetController petController)
        {
            this.blockData = blockData;
            this.petController = petController;
            selectedTime = Time.time;

            this.petController.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 6f);
            this.petController.SetToIdle(6f);

            var petGameType = BlockStatusManager.Instance.GetGameType(blockData.type);
            if (petGameType != GameType.Null) this.petController.OnGameEnter(petGameType);
        }

        public void EnterGame(GameType type)
        {
            if (blockData == null) enterGameWithPet = false;
            else enterGameWithPet = blockData.type.ToString() == type.ToString() && Time.time < selectedTime + 6.5f;

            switch (type)
            {
                case GameType.build:
                    build.SetPlayer(enterGameWithPet, petController);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.land:
                    land.SetPlayer(enterGameWithPet, petController);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.jump:
                    jump.SetPlayer(enterGameWithPet, petController);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.shoot:
                    shoot.SetPlayer(enterGameWithPet, petController);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
            }

            gameType = type;
        }

        public void ExitGame()
        {
            if (!enterGameWithPet) return;

            petController.OnGameExit(gameType);
            petController.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 2f);
            petController.SetToIdle(2f);
        }
    }
}