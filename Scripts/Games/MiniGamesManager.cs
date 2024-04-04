using System.Collections.Generic;
using Core.Pet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Games
{
    public enum GameType
    {
        land,
        jump,
        build,
        shoot,
        @null
    }

    public class MiniGamesManager : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<GameType, MiniGameManager> gameManagers;

        public static MiniGamesManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void EnterGame(GameType gameType)
        {
            gameManagers[gameType].OnGameEnter();
        }

        public void ExitGame(GameType gameType)
        {
            gameManagers[gameType].ReturnToMenu();
        }

        public void RestartGame(GameType gameType)
        {
            gameManagers[gameType].RestartGame();
        }

        public void SetupPet(GameType gameType, bool isPlayingWithPet, PetObject petObject = null)
        {
            gameManagers[gameType].SetupPet(isPlayingWithPet, petObject);
        }
    }
}