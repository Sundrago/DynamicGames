using DynamicGames.MiniGames;
using DynamicGames.MiniGames.Shoot;
using DynamicGames.System;
using UnityEngine;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Manages the pet behaviour in the game.
    /// </summary>
    public class PetInGameManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager shoot;
        [SerializeField] private MiniGames.Jump.GameManager jump;
        [SerializeField] private MiniGames.Land.GameManager land;
        [SerializeField] private MiniGames.Build.GameManager build;
        [SerializeField] public PetObject petObject;

        private BlockStatusManager.BlockStatusData blockData;
        private GameType gameType;
        private float selectedTime;
        public bool EnterGameWithPet { get; private set; }

        public static PetInGameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void PetSelected(BlockStatusManager.BlockStatusData blockData, PetObject petObject)
        {
            this.blockData = blockData;
            this.petObject = petObject;
            selectedTime = Time.time;

            this.petObject.petSurfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 6f);
            this.petObject.SetToIdle(6f);

            var petGameType = BlockStatusManager.Instance.GetGameType(blockData.type);
            if (petGameType != GameType.@null) this.petObject.OnGameEnter(petGameType);
        }

        public void EnterGame(GameType gameType)
        {
            if (blockData == null) EnterGameWithPet = false;
            else EnterGameWithPet = blockData.type.ToString() == gameType.ToString() && Time.time < selectedTime + 6.5f;

            this.gameType = gameType;
            MiniGamesManager.Instance.SetupPet(gameType, EnterGameWithPet, petObject);
            TutorialManager.Instancee.EnteredGameWithPet();
        }

        public void ExitGame()
        {
            if (!EnterGameWithPet) return;

            petObject.OnGameExit(gameType);
            petObject.petSurfaceMovement2D.ForceLandOnSquare(blockData.obj.blockDragHandler.miniisland, 2f);
            petObject.SetToIdle(2f);
        }
    }
}