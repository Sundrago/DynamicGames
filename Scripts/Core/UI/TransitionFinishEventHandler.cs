using Games.Build;
using UnityEngine;

namespace Core.UI
{
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
                    eventHandler.GetComponent<Games.Land.GameManager>().OnGameEnter();
                    break;
                case "jump":
                    eventHandler.GetComponent<Games.Jump.GameManager>().OnGameEnter();
                    break;
            }
        }
    }
}