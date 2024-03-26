using UnityEngine;

namespace Games.Jump
{
    /// <summary>
    ///     A class that holds data for the footstep object in a jump game.
    /// </summary>
    public class FootstepObject : MonoBehaviour
    {
        public float Height { get; private set; }
        public Vector2 Pos { get; private set; }

        private void Start()
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            Pos = rectTransform.position;
            Height = rectTransform.sizeDelta.y;
        }
    }
}