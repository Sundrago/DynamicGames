using Core.Pet;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games
{
    public abstract class MiniGame : SerializedMonoBehaviour
    {
        [FormerlySerializedAs("ReturnToMenuButton")] [SerializeField] private ReturnToMenu returnToMenuButton;
        public abstract void OnGameEnter();
        public abstract void ClearGame();
        public abstract void RestartGame();
        public abstract void SetPlayer(bool playAsPet, PetController petController = null);

        public void ReturnToMenu()
        {
            returnToMenuButton.ReturnToMenuClkcked();
        }
    }
}