using UnityEngine;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     Handles footstep physics for jumping.
    /// </summary>
    public class FootstepScrollController : MonoBehaviour
    {
        public float ScrollSpeed { get; } = -1;

        private void Update()
        {
            gameObject.transform.Translate(ScrollSpeed * Time.deltaTime * Vector3.up);
        }
    }
}