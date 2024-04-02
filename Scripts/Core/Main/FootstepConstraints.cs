using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Main
{
    /// <summary>
    /// Controls the footstep constraints for a stepable object.
    /// </summary>
    public class FootstepConstraints : MonoBehaviour
    {
        [SerializeField] public bool[] constraints = new bool[4];
    }
}