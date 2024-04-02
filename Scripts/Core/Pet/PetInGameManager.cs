using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Core.Pet
{
    public class PetInGameManager : MonoBehaviour
    {
        public static PetInGameManager Instance;

        [SerializeField] private Games.Shoot.GameManager shoot;
        [SerializeField] private Games.Jump.GameManager jump;
        [SerializeField] private Games.Land.GameManager land;
        [SerializeField] private Games.Build.GameManager build;

        private BlockStatusManager.BlockStatusData blockData = null;
        public Pet pet = null;
        private float selectedTime;
        public bool enterGameWithPet = false;
        private GameType gameType;

        public bool EnterGameWithPet => enterGameWithPet;

        private void Awake()
        {
            Instance = this;
        }

        public void PetSelected(BlockStatusManager.BlockStatusData _blockData, Pet _pet)
        {
            blockData = _blockData;
            pet = _pet;
            selectedTime = Time.time;

            pet.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 6f);
            pet.SettoIdle(6f);

            GameType petGameType = BlockStatusManager.Instance.GetGameType(blockData.type);
            if (petGameType != GameType.Null) pet.OnGameEnter(petGameType);
        }

        public void EnterGame(GameType type)
        {
            if (blockData == null) enterGameWithPet = false;
            else enterGameWithPet = (blockData.type.ToString() == type.ToString() && Time.time < selectedTime + 6.5f);

            switch (type)
            {
                case GameType.build:
                    build.SetPlayer(enterGameWithPet, pet);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.land:
                    land.SetPlayer(enterGameWithPet, pet);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.jump:
                    jump.SetPlayer(enterGameWithPet, pet);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
                case GameType.shoot:
                    shoot.SetPlayer(enterGameWithPet, pet);
                    TutorialManager.Instancee.EnteredGameWithPet();
                    break;
            }

            // if(pet!=null && enterGameWithPet) pet.OnGameEnter(type);
            gameType = type;
        }

        public void ExitGame()
        {
            if (!enterGameWithPet) return;

            pet.OnGameExit(gameType);
            pet.surfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 2f);
            pet.SettoIdle(2f);
        }

    }
}