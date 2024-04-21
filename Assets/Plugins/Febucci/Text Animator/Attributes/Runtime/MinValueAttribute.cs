using UnityEngine;

namespace Febucci.Attributes
{
    public class MinValueAttribute : PropertyAttribute
    {
        public float min;

        public MinValueAttribute(float min)
        {
            this.min = min;
        }
    }
}