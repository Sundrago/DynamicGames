using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_playerCtrl : MonoBehaviour
{
    [SerializeField] float jumpforce;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("footstep") & gameObject.GetComponent<Rigidbody2D>().velocity.y <= 0) {
            print("footstep");

            Vector2 newPos = new Vector2(gameObject.transform.localPosition.x, other.transform.localPosition.y);
            newPos.y += 35.5f;
            gameObject.transform.localPosition = newPos;
            //gameObject.GetComponent<RectTransform>().transform.local
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0,jumpforce));
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        OnCollisionEnter2D(other);
    }
}
