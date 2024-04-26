using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// Responsible for handling animation end events and triggering appropriate actions in the Land GameManager.
    public class AnimationEndHandler : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private string source;

        public void AnimEndEvent()
        {
            switch (source)
            {
                case "rocket":
                    gameManager.RocketIsReady();
                    break;
                case "clear":
                    gameManager.ResetRocket();
                    break;
            }
        }

        public void AnimEndEvent2()
        {
            switch (source)
            {
                case "rocket":
                    gameManager.OpenNextStage();
                    break;
            }
        }
    }
}