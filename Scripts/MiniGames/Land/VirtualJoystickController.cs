using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Represents a virtual joystick controller in a game.
    /// </summary>
    public class VirtualJoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public enum GameControls
        {
            Up,
            Left,
            Right
        }

        public enum JoystickState
        {
            Idle,
            Left,
            Up,
            Right,
            Down
        }

        [SerializeField] private GameManager gameManager;
        [SerializeField] private GameObject joystickUI;
        [SerializeField] private Sprite[] joystickImg = new Sprite[4];

        private int currentStateIndex;
        private Vector2 initialPoint;
        private float joystickHalfScaleY;

        private void Start()
        {
            SetState(JoystickState.Idle);
            joystickUI.SetActive(false);
            joystickHalfScaleY = joystickUI.GetComponent<RectTransform>().transform.localScale.y / 2f;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var degree = Mathf.Atan2(initialPoint.y - mousePos.y, initialPoint.x - mousePos.x) * Mathf.Rad2Deg;
            var dist = Vector2.Distance(initialPoint, mousePos);

            SetState(CalcJoystickState(degree, dist));
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SetState(JoystickState.Idle);
            initialPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            joystickUI.transform.position = new Vector2(initialPoint.x, initialPoint.y + joystickHalfScaleY);
            joystickUI.SetActive(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            SetState(JoystickState.Idle);
            joystickUI.SetActive(false);
        }

        private void SetState(JoystickState state)
        {
            var stateIndex = (int)state;
            if (currentStateIndex != stateIndex)
            {
                currentStateIndex = stateIndex;
                joystickUI.GetComponent<Image>().sprite = joystickImg[stateIndex];
            }

            UpdateGameManagerButtons(state);
        }

        private void UpdateGameManagerButtons(JoystickState state)
        {
            switch (state)
            {
                case JoystickState.Idle:
                    SetGameManagerButtons(false, false, false);
                    break;
                case JoystickState.Left:
                    SetGameManagerButtons(false, true, false);
                    break;
                case JoystickState.Up:
                    SetGameManagerButtons(true, false, false);
                    break;
                case JoystickState.Right:
                    SetGameManagerButtons(false, false, true);
                    break;
            }
        }

        private void SetGameManagerButtons(bool up, bool left, bool right)
        {
            gameManager.ChangeButtonState(nameof(GameControls.Up), up);
            gameManager.ChangeButtonState(nameof(GameControls.Left), left);
            gameManager.ChangeButtonState(nameof(GameControls.Right), right);
        }

        private JoystickState CalcJoystickState(float degree, float distance)
        {
            if (distance < joystickHalfScaleY)
                return JoystickState.Idle;
            if ((degree > -60f) & (degree < 0)) return JoystickState.Left;
            if ((degree > -120f) & (degree < -60f)) return JoystickState.Up;
            if ((degree > -180f) & (degree < -120f)) return JoystickState.Right;
            return JoystickState.Idle;
        }
    }
}