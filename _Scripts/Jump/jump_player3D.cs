using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jump_player3D : MonoBehaviour
{
    [SerializeField] Jump_StageCtrl jumpStage;
    [SerializeField] SFXCTRL sfx;
    [SerializeField] GameObject hitFX, footstepHolder;

    [SerializeField] float jumpforce, posYAdd;
    [SerializeField] float holdTime;
    [SerializeField]
    private SpriteAnimator spriteAnimator;
    
    void Update()
    {
        if(Time.frameCount % 5 != 0) return;

        if(Mathf.Abs(GetComponent<Rigidbody>().velocity.y) >= 0.01f)
            holdTime = Time.time;

        if(Time.time - holdTime > 1f)
            GetComponent<Rigidbody>().AddForce(new Vector3(0,jumpforce,0));
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("footstep") & gameObject.GetComponent<Rigidbody>().velocity.y <= 0 &
            gameObject.transform.position.y > other.transform.position.y) {

            Vector3 newPos = gameObject.transform.position;
            newPos.y = other.gameObject.transform.position.y;
            newPos.y += posYAdd;
            gameObject.transform.position = newPos;

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(new Vector3(0,jumpforce,0));

            spriteAnimator.RestartWithNoLoop();
            
            int footidx = -1;
            if(int.TryParse(other.gameObject.transform.parent.gameObject.name, out footidx)) {
                if(footidx > jumpStage.score) {
                    jumpStage.score = footidx;
                    jumpStage.UpdateScoreUI(footidx);
                    GameObject obj = Instantiate(hitFX, footstepHolder.transform);
                    obj.transform.localPosition = other.transform.parent.transform.localPosition;
                    obj.SetActive(true);
                }
            }
            sfx.WaterSfx();
        }
    }

    void OnCollisionStay(Collision other)
    {
        OnCollisionEnter(other);
    }
}
