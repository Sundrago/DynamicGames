using UnityEngine;

namespace Games.Jump
{
    /// <summary>
    ///     Class that handles footstep physics for jumping.
    /// </summary>
    public class FootstepScrollController : MonoBehaviour
    {
        public float ScrollSpeed { get; private set; } = -1;

        private void Update()
        {
            gameObject.transform.Translate(ScrollSpeed * Time.deltaTime * Vector3.up);
        }
    }
}