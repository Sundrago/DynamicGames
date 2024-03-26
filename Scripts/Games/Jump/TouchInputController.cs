using UnityEngine;
using UnityEngine.EventSystems;

namespace Games.Jump
{
    /// <summary>
    ///     Controls touch input for the jump game.
    /// </summary>
    public class TouchInputController : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject cylindar;
        [SerializeField] private float rotationSpeed = 0.5f;

        private Vector2 previousTouchPosition;

        public void OnDrag(PointerEventData eventData)
        {
            var touchDelta = eventData.position - previousTouchPosition;
            var rotationAmount = -touchDelta.x * rotationSpeed * Time.deltaTime;

            cylindar.transform.Rotate(0f, rotationAmount, 0f, Space.Self);
            previousTouchPosition = eventData.position;

            if (touchDelta.x > 2f) player.transform.localScale = new Vector3(1, 1, 1);
            else if (touchDelta.x < -2f) player.transform.localScale = new Vector3(-1, 1, 1);
            ;
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