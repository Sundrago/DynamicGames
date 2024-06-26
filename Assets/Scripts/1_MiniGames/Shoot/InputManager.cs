using DynamicGames.System;
using DynamicGames.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages the user input for the shooting mini-game.
    /// </summary>
    public class InputManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float MaxRadius = 75f;
        private const float MaxVelocity = 150f;
        private const float MoveSpeed = 10f;
        private const float Friction = 0.85f;
        private const float MaxSpeed = 5f;
        private const float Velocity = 0.2f;

        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BulletManager bullet_Manager;

        [Header("Game Components")] 
        [SerializeField] private GameObject joystickUI;
        [SerializeField] private GameObject joystickKnob;
        [SerializeField] private GameObject targetObject;
        [SerializeField] private Boundaries boundaries;
        [SerializeField] private SpriteAnimator player;
        
        private Vector2 initialPoint;
        private float joystickSensitivity = 1;
        private bool onDrag;
        private Vector3 speed = Vector3.zero;

        public Vector3 NormalVector { get; private set; }

        private void Start()
        {
            joystickUI.SetActive(false);
            NormalVector = Vector3.zero;
            player.PauseAnim();
        }

        private void LateUpdate()
        {
            CalculateSpeed();
            UpdatePosition();
            AdjustPositionWithinBoundaries();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!onDrag) return;
            Vector2 dragPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            UpdatePointer(dragPoint);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!(gameManager.state == GameManager.ShootGameState.Playing ||
                  gameManager.state == GameManager.ShootGameState.Ready!))
            {
                ResetJoystick();
                return;
            }

            onDrag = true;
            initialPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            joystickUI.transform.position = initialPoint;
            joystickUI.SetActive(true);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            ResetJoystick();
            player.PauseAnim();
        }

        private void CalculateSpeed()
        {
            if (onDrag) speed += NormalVector * Velocity * joystickSensitivity;
            speed *= Friction;
            speed = new Vector3(Mathf.Clamp(speed.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(speed.y, -MaxSpeed, MaxSpeed),
                0);
        }

        private void UpdatePosition()
        {
            targetObject.transform.position += speed * Time.deltaTime;
        }

        private void AdjustPositionWithinBoundaries()
        {
            var position = targetObject.transform.position;
            if (position.x < boundaries.left.position.x)
                position.x = boundaries.left.position.x;
            else if (position.x > boundaries.right.position.x)
                position.x = boundaries.right.position.x;

            if (position.y > boundaries.top.position.y)
                position.y = boundaries.top.position.y;
            else if (position.y < boundaries.btm.position.y)
                position.y = boundaries.btm.position.y;


            targetObject.transform.position = position;
        }

        public void ResetJoystick()
        {
            onDrag = false;
            joystickKnob.transform.localPosition = Vector2.zero;
            joystickUI.SetActive(false);
        }

        private void UpdatePointer(Vector2 dragPoint)
        {
            if (!onDrag) return;

            if (!(gameManager.state == GameManager.ShootGameState.Playing ||
                  gameManager.state == GameManager.ShootGameState.Ready!))
            {
                ResetJoystick();
                return;
            }

            CalculateJoystickMovement(dragPoint);
            UpdatePlayerAnimation();
        }

        private void CalculateJoystickMovement(Vector2 dragPoint)
        {
            var vec = new Vector2(dragPoint.x - initialPoint.x, dragPoint.y - initialPoint.y);
            vec = Vector2.ClampMagnitude(vec * MaxVelocity, MaxRadius);
            joystickKnob.transform.localPosition = vec;
            NormalVector = vec.normalized;

            var angle = Mathf.Atan2(NormalVector.y, NormalVector.x) * Mathf.Rad2Deg;
            targetObject.transform.rotation = Quaternion.Lerp(targetObject.transform.rotation,
                Quaternion.Euler(new Vector3(0f, 0f, angle)), 0.5f);

            joystickSensitivity = Vector2.Distance(Vector2.zero, joystickKnob.transform.localPosition) / 50f;
        }

        private void UpdatePlayerAnimation()
        {
            if (NormalVector.x + NormalVector.y > 0.2f)
                player.ResumeAnim();
            else
                player.PauseAnim();
        }
    }
}