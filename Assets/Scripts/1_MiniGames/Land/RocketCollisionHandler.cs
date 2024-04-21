using UnityEngine;

namespace DynamicGames.MiniGames.Land
{
    /// <summary>
    ///     Handles rocket collision detection and response.
    /// </summary>
    public class RocketCollisionHandler : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private string collisionSource;

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collisionSource == "island") return;
            gameManager.ChangeColliderState(collisionSource, true);
            gameManager.failPosition = collision2D.contacts[0].point;
            gameManager.failRotation = collision2D.transform.rotation;
        }

        private void OnCollisionExit2D(Collision2D collision2D)
        {
            if (collisionSource == "island") return;
            gameManager.ChangeColliderState(collisionSource, false);
        }

        private void OnTriggerEnter2D(Collider2D collision2D)
        {
            if (collisionSource == "island")
                switch (collision2D.gameObject.name)
                {
                    case "landing_left":
                        gameManager.ChangeColliderState("left", true);
                        break;
                    case "landing_right":
                        gameManager.ChangeColliderState("right", true);
                        break;
                }
        }
    }
}