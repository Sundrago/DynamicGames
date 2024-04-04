using UnityEngine;

namespace DynamicGames.System
{
    /// <summary>
    /// Responsible for destroying the game object it is attached to.
    /// </summary>
    public class DestroySelf : MonoBehaviour
    {
        public void DestroySelfObj()
        {
            Destroy(gameObject);
        }
    }
}