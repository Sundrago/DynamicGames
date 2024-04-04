using DynamicGames.MiniGames.Build;
using UnityEngine;

namespace DynamicGames.UI
{
    /// <summary>
    /// Handles the transition finish event and performs necessary actions based on the game type.
    /// </summary>
    public class TransitionFinishEventHandler : MonoBehaviour
    {
        [SerializeField] private GameObject eventHandler;
        [SerializeField] private string type;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void GameReady()
        {
            switch (type)
            {
                case "build":
                    eventHandler.GetComponent<GameManager>().OnGameEnter();
                    break;
                case "rocket":
                    eventHandler.GetComponent<MiniGames.Land.GameManager>().OnGameEnter();
                    break;
                case "jump":
                    eventHandler.GetComponent<MiniGames.Jump.GameManager>().OnGameEnter();
                    break;
            }
        }
    }
}