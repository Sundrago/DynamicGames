using System;
using Core.Pet;
using UnityEngine;

namespace Games
{
    /// <summary>
    /// Interface for a mini game.
    /// </summary>
    public interface IMiniGame
    {
        void OnGameEnter();
        void ClearGame();
        void RestartGame();
        void SetupPet(bool isPlayingWithPet, PetObject petObject = null);
    }
}