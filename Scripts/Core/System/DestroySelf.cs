using UnityEngine;

namespace Core.System
{
    public class DestroySelf : MonoBehaviour
    {
        public void DestroySelfObj()
        {
            Destroy(gameObject);
        }
    }
}