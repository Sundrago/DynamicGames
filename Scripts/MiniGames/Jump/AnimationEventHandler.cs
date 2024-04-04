using UnityEngine;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     Handles animation events for the jump game.
    /// </summary>
    public class AnimationEventHandler : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Preload()
        {
            gameManager.ClearGame();
        }

        public void Ready()
        {
            gameManager.OnGameEnter();
        }
    }
}