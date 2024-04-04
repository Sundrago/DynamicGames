using Core.Pet;
using Core.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games
{
    public abstract class MiniGameManager : SerializedMonoBehaviour, IMiniGame
    {
        [FormerlySerializedAs("ReturnToMenuButton")] [SerializeField] private ReturnToMenu returnToMenuButton;
        public abstract void OnGameEnter();
        public abstract void ClearGame();
        public abstract void RestartGame();
        public abstract void SetupPet(bool isPlayingWithPet, PetObject petObject = null);

        public void ReturnToMenu()
        {
            returnToMenuButton.OnReturnToMenuButtonClick();
        }
    }
}