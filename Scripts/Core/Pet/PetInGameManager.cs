using Core.System;
using Games;
using Games.Shoot;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("petController")] [SerializeField] public PetObject petObject;
        
        private BlockStatusManager.BlockStatusData blockData;
        private GameType gameType;
        private float selectedTime;
        private bool enterGameWithPet;
        public bool EnterGameWithPet => enterGameWithPet;

        private void Awake()
        {
            Instance = this;
        }

        public void PetSelected(BlockStatusManager.BlockStatusData blockData, PetObject petObject)
        {
            this.blockData = blockData;
            this.petObject = petObject;
            selectedTime = Time.time;

            this.petObject.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 6f);
            this.petObject.SetToIdle(6f);

            var petGameType = BlockStatusManager.Instance.GetGameType(blockData.type);
            if (petGameType != GameType.@null) this.petObject.OnGameEnter(petGameType);
        }

        public void EnterGame(GameType gameType)
        {
            if (blockData == null) enterGameWithPet = false;
            else enterGameWithPet = blockData.type.ToString() == gameType.ToString() && Time.time < selectedTime + 6.5f;

            this.gameType = gameType;
            MiniGamesManager.Instance.SetupPet(gameType, enterGameWithPet, petObject);
            TutorialManager.Instancee.EnteredGameWithPet();
            
            
            // switch (type)
            // {
            //     case GameType.Build:
            //         build.SetupPet(enterGameWithPet, petObject);
            //         TutorialManager.Instancee.EnteredGameWithPet();
            //         break;
            //     case GameType.Land:
            //         land.SetupPet(enterGameWithPet, petObject);
            //         TutorialManager.Instancee.EnteredGameWithPet();
            //         break;
            //     case GameType.Jump:
            //         jump.SetupPet(enterGameWithPet, petObject);
            //         TutorialManager.Instancee.EnteredGameWithPet();
            //         break;
            //     case GameType.Shoot:
            //         shoot.SetupPet(enterGameWithPet, petObject);
            //         TutorialManager.Instancee.EnteredGameWithPet();
            //         break;
            // }

        }

        public void ExitGame()
        {
            if (!enterGameWithPet) return;

            petObject.OnGameExit(gameType);
            petObject.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 2f);
            petObject.SetToIdle(2f);
        }
    }
}