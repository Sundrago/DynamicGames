using DynamicGames.Pet;

namespace DynamicGames.MiniGames
{
    /// <summary>
    ///     Interface for a mini-games.
    /// </summary>
    public interface IMiniGame
    {
        void OnGameEnter();
        void ClearGame();
        void RestartGame();
        void SetupPet(bool isPlayingWithPet, PetObject petObject = null);
    }
}