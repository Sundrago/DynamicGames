public interface IMiniGame
{
    // Method called when entering the game.
    void OnGameEnter();
    
    // Clears the current game state by destroying all game objects, clearing lists, and resetting UI components.
    void ClearGame();
    
    // Restarts the game by clearing the current game state and reloading the stage.
    void RestartGame();
}
