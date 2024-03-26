using UnityEngine;

namespace Games.Jump
{
    /// <summary>
    ///     Controls the movement and behavior of a player in a 2D environment.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        private const float JumpHeight = 35.5f;
        [SerializeField] private float jumpForce;
        private Rigidbody2D rigidBody;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (IsFootstepCollisionTriggered(other))
                PerformJump(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (IsFootstepCollisionTriggered(other))
                PerformJump(other);
        }

        private bool IsFootstepCollisionTriggered(Collision2D other)
        {
            var rigidBodyVelocity = rigidBody.velocity;
            return other.gameObject.CompareTag("footstep") && rigidBodyVelocity.y <= 0;
        }

        private void PerformJump(Collision2D other)
        {
            var newPos = new Vector2(gameObject.transform.localPosition.x, other.transform.localPosition.y);
            newPos.y += JumpHeight;

            gameObject.transform.localPosition = newPos;
            rigidBody.velocity = Vector2.zero;
            rigidBody.AddForce(new Vector2(0, jumpForce));
        }
    }
}