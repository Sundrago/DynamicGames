using UnityEngine;

namespace DynamicGames.System
{
    /// <summary>
    ///     Holds the boundaries of a game area.
    /// </summary>
    public class Boundaries : MonoBehaviour
    {
        [SerializeField] public Transform top, left, right, btm;
    }
}