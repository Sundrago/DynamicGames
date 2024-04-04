using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// This class handles the events when a button is being held down.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonHoldHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button button;
        private bool isMouseDown;
        private float startTime;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        private void Update()
        {
            if (!isMouseDown || !button.interactable) return;
            if (startTime + 0.5f > Time.time) return;

            if (startTime + 2f > Time.time)
            {
                if (Time.frameCount % 8 == 0)
                    button.onClick.Invoke();
            }
            else if (startTime + 4f > Time.time)
            {
                if (Time.frameCount % 4 == 0)
                    button.onClick.Invoke();
            }
            else if (Time.frameCount % 2 == 0)
            {
                button.onClick.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            startTime = Time.time;
            isMouseDown = true;
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            isMouseDown = false;
        }
    }
}