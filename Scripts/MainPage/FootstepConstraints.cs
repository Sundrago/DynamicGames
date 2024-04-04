using UnityEngine;

namespace DynamicGames.MainPage
{
    /// <summary>
    ///     Holds footstep constraints data for a stepable object.
    /// </summary>
    public class FootstepConstraints : MonoBehaviour
    {
        [SerializeField] public bool[] constraints = new bool[4];
    }
}