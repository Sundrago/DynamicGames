using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_anim : MonoBehaviour
{
    [SerializeField] Sprite[] walks;
    [SerializeField] Sprite[] jumps;
    [SerializeField] int framerate = 3;

    private int walksidx = 0;
    private bool walk;
    private IEnumerator coroutine;
    
    public void Jump(){
        walk = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = jumps[0];
    }

    public void Walk() {
        //print("walk");
        walk = true;
        StartCoroutine(WalkAnim());
    }

    IEnumerator WalkAnim() {
        //print("walk anim");
        if(!walk) yield break;

        walksidx += 1;
        if(walksidx >= walks.Length) walksidx = 0;

        if(gameObject.GetComponent<SpriteRenderer>().sprite != walks[walksidx]) {
            gameObject.GetComponent<SpriteRenderer>().sprite = walks[walksidx];
        } else {
            gameObject.GetComponent<SpriteRenderer>().sprite = walks[walksidx];
        }

        yield return new WaitForSeconds(1f / framerate);
        StartCoroutine("WalkAnim");
        yield break;
    }
}
