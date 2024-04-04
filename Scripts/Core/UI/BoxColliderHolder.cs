using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    public class BoxColliderHolder : MonoBehaviour
    {
        [SerializeField] public BoxCollider2D[] squares;

#if UNITY_EDITOR
        [Button]
        private void AddSquaresToList()
        {
            var objs = GameObject.FindGameObjectsWithTag("square");
            squares = new BoxCollider2D[objs.Length];

            for (var i = 0; i < objs.Length; i++) squares[i] = objs[i].GetComponent<BoxCollider2D>();
        }
#endif
    }
}