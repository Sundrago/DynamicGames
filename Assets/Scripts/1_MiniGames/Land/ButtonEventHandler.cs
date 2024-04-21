using UnityEngine;
using UnityEngine.EventSystems;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Handles button events for the Land game.
    /// </summary>
    public class ButtonEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private GameManager rocket;
        public string buttonSource;

        public void OnPointerDown(PointerEventData eventData)
        {
            rocket.ChangeButtonState(buttonSource, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rocket.ChangeButtonState(buttonSource, false);
        }
    }
}