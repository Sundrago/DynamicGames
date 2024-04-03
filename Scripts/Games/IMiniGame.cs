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
        /// <summary>
        /// This method is called when the game starts or a new game is initiated.
        /// </summary>
        void OnGameEnter();

        /// <summary>
        /// Clears the game by removing all stage items, resetting game status, and hiding UI elements.
        /// </summary>
        void ClearGame();

        /// <summary>
        /// Restarts the game by clearing the game state, resetting the UI components, and reloading the stage.
        /// </summary>
        void RestartGame();

        /// <summary>
        /// Sets the player for the mini game.
        /// </summary>
        /// <param name="playAsPet">If true, the player will play as a pet.</param>
        /// <param name="petController">The pet object to be set as the player. Optional parameter, default is null.</param>
        void SetPlayer(bool playAsPet, PetController petController = null);
    }
}