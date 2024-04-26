using UnityEngine;
using UnityEngine.EventSystems;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     Controls touch input for the jump game.
    /// </summary>
    public class TouchInputController : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private const float RotationSpeed = 40f;
        
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject cylindar;

        private Vector2 previousTouchPosition;

        public void OnDrag(PointerEventData eventData)
        {
            var touchDelta = eventData.position - previousTouchPosition;
            var rotationAmount = -touchDelta.x * RotationSpeed * Time.deltaTime;

            cylindar.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
            previousTouchPosition = eventData.position;

            if (touchDelta.x > 2f) player.transform.localScale = new Vector3(1, 1, 1);
            else if (touchDelta.x < -2f) player.transform.localScale = new Vector3(-1, 1, 1);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            previousTouchPosition = eventData.position;
        }

        public void ResetRotation()
        {
            cylindar.transform.localEulerAngles = Vector3.zero;
        }
    }
}