 using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Controls the background animation shooting mini-game.
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