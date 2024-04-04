using UnityEngine;

namespace DynamicGames.MainPage
{
    /// <summary>
    ///     Controls the footstep constraints for a stepable object.
    /// </summary>
    public class FootstepConstraints : MonoBehaviour
    {
        [SerializeField] public bool[] constraints = new bool[4];
    }
}