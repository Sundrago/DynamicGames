using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Land
{
    /// <summary>
    /// Handles rocket collision detection and response.
    /// </summary>
    public class RocketCollisionHandler : MonoBehaviour
    {
        [SerializeField] private GameManager rocket;
        [SerializeField] private string collisionSource;

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collisionSource == "island") return;
            rocket.ChangeColliderState(collisionSource, true);
            rocket.failPosition = collision2D.contacts[0].point;
            rocket.failRotation = collision2D.transform.rotation;
        }

        private void OnCollisionExit2D(Collision2D collision2D)
        {
            if (collisionSource == "island") return;
            rocket.ChangeColliderState(collisionSource, false);
        }

        private void OnTriggerEnter2D(Collider2D collision2D)
        {
            if (collisionSource == "island")
                switch (collision2D.gameObject.name)
                {
                    case "landing_left":
                        rocket.ChangeColliderState("left", true);
                        break;
                    case "landing_right":
                        rocket.ChangeColliderState("right", true);
                        break;
                }
        }
    }
}