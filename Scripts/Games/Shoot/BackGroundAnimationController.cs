using UnityEngine;

namespace Games.Shoot
{
    /// <summary>
    ///     Controls the background animation in the shoot game.
    /// </summary>
    public class BackGroundAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float offsetAmount;
        private Vector3 originalPos;

        private void Start()
        {
            originalPos = gameObject.transform.position;
        }

        private void Update()
        {
            gameObject.transform.position = player.transform.position * offsetAmount;
        }
    }
}