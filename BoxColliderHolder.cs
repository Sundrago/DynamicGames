using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BoxColliderHolder : MonoBehaviour
{
    [SerializeField] public BoxCollider2D[] squares;
    
#if UNITY_EDITOR
    [Button]
    private void AddSquaresToList()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("square");
        squares = new BoxCollider2D[objs.Length];
        
        for(int i=0; i<objs.Length; i++)
        {
            squares[i] = objs[i].GetComponent<BoxCollider2D>();
        }
    }
#endif
}
