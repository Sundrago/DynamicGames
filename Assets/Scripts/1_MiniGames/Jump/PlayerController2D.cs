using UnityEngine;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     [Deprecated] Controls the movement and behavior of a player in a 2D environment.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        private const float JumpForce = 0.8f;
        private const float JumpHeight = 35.5f;
        private Rigidbody2D rigidBody;

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (IsFootstepCollisionTriggered(collision2D))
                PerformJump(collision2D);
        }

        private void OnCollisionStay2D(Collision2D collision2D)
        {
            if (IsFootstepCollisionTriggered(collision2D))
                PerformJump(collision2D);
        }

        private bool IsFootstepCollisionTriggered(Collision2D collision2D)
        {
            var rigidBodyVelocity = rigidBody.velocity;
            return collision2D.gameObject.CompareTag("footstep") && rigidBodyVelocity.y <= 0;
        }

        private void PerformJump(Collision2D other)
        {
            var newPos = new Vector2(gameObject.transform.localPosition.x, other.transform.localPosition.y);
            newPos.y += JumpHeight;

            gameObject.transform.localPosition = newPos;
            rigidBody.velocity = Vector2.zero;
            rigidBody.AddForce(new Vector2(0, JumpForce));
        }
    }
}