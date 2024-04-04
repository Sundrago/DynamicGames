using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Land
{
    public class AnimationEndHandler : MonoBehaviour
    {
        [FormerlySerializedAs("gameManagerManager")] [SerializeField] private GameManager gameManager;
        [FormerlySerializedAs("idx")] public string source;

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