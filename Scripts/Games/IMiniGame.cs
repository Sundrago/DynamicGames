using Core.Pet;

namespace Games
{
    /// <summary>
    /// Interface for a mini game.
    /// </summary>
    public interface IMiniGame
    {
        /// <summary>
        /// This method is called when the game starts or a new game is initiated.
        /// It resets the game state, calculates the initial hit height, and shows the tutorial if necessary.
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
        /// <param name="pet">The pet object to be set as the player. Optional parameter, default is null.</param>
        void SetPlayer(bool playAsPet, Pet pet = null);
    }
}