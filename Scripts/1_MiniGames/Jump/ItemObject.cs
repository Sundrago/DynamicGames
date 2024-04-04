using UnityEngine;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     A simple rotation fx for jump item.
    /// </summary>
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed = new(0, 100, 0);

        private void Update()
        {
            RotateObject();
        }

        private void RotateObject()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}