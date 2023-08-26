using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class DragSprite : MonoBehaviour
{
    [SerializeField] TextMeshPro[] title;
    [SerializeField] GameObject UIFX;
    [SerializeField] GameObject myUIFX;
    [SerializeField] MainCanvas main;

    [SerializeField] string btnName;
    [SerializeField] bool useForce = false;

    public bool btnSelected = false;
    public GameObject miniisland;

    private Vector2 startPosition;
    private Color transparent = Color.white;
    private Vector3 initialPos, initialRotation;

    private bool drag = false;
    private bool showtitle = false;
    private float mouseDownTime;

    void Start()
    {
        initialPos = gameObject.transform.position;
        initialRotation = gameObject.transform.eulerAngles;

        transparent.a = 0;
        if(title.Length > 0) {
            foreach(TextMeshPro text in title) {
                text.color = transparent;
                text.gameObject.SetActive(false);
            }
        }
    }

   public void OnMouseDown()
    {
        main.Offall(gameObject);
        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        startPosition = eventDataCurrentPosition.position;

        drag = true;
        mouseDownTime = Time.time;

        if(gameObject.GetComponent<Rigidbody2D>() != null) {
            if(!useForce) gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }

        if(UIFX != null) {
            if(myUIFX != null) {
                myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
            }
            myUIFX = Instantiate(UIFX, UIFX.transform.parent.transform);
            myUIFX.GetComponent<EnergyUIFXCtrl>().InitiateFX(gameObject);
        }
    }

    void OnMouseDrag()
    {
        if (drag)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(!useForce) {
                this.transform.position = Vector2.Lerp(this.transform.position, mousePosition, 0.3f); 
                if(Time.time - mouseDownTime > 0.5f) {

                    ShowTitle();

                    float targetAngle = this.transform.eulerAngles.z % 360;
                    if(targetAngle < 180) targetAngle = this.transform.eulerAngles.z - this.transform.eulerAngles.z % 360;
                    else targetAngle = this.transform.eulerAngles.z + (360 - this.transform.eulerAngles.z % 360);
                    this.transform.eulerAngles = Vector3.Lerp(gameObject.transform.localEulerAngles, new Vector3(0,0,targetAngle), 0.1f);
                }
            }
            else {
                Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
                gameObject.GetComponent<Rigidbody2D>().AddForce (direction * Time.deltaTime * 1000f);
            }
        }
    }

    private void OnMouseUp()
    {
        if(gameObject.GetComponent<Rigidbody2D>() != null) {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            HideTitle();
            if(!useForce) gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
            if(Time.time - mouseDownTime > 1f) {
                if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
                DOVirtual.Float(0f,1f,0.5f,GravityUpdate).SetEase(Ease.InQuart);
            }
        }
        if (drag & Vector2.Distance(startPosition, new Vector2(Input.mousePosition.x, Input.mousePosition.y)) < 15f 
        || Vector3.Distance(gameObject.transform.position, new Vector3(0f,1f,0f)) < 0.75f) {
            BtnClicked();
            print("backclicked");
        }
        else if(myUIFX != null) {
            myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
        }
    }

    void GravityUpdate(float gravity) {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = gravity;
    }

    void ShowTitle(){
        if(showtitle) return;

        if(title.Length <= 0) return;
        // transparent.a = 0f;
        foreach(TextMeshPro text in title) {
            // text.color = transparent;
            text.gameObject.SetActive(true);
            if(DOTween.IsTweening(text)) DOTween.Kill(text);
            text.DOFade(1, 1f)
            .SetEase(Ease.OutQuart);
        }
        showtitle = true;
    }

    void HideTitle(){
        showtitle = false;
        if(title.Length <= 0) return;
        // transparent.a = 1f;
        foreach(TextMeshPro text in title) {
            // text.color = transparent;
            if(DOTween.IsTweening(text)) DOTween.Kill(text);
            text.DOFade(0, 0.5f)
            .SetEase(Ease.InQuart)
            .OnComplete(()=>{text.gameObject.SetActive(false);});
        }
    }

    void BtnClicked() {
        if(btnSelected) {
            main.GotoGame(btnName, gameObject);
            return;
        }
        print("showtitle " + showtitle);

        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.transform.DOMove(new Vector3(-0.4f,0.9f,0f), 2f)
        .SetEase(Ease.OutBack);
        gameObject.transform.DORotate(new Vector3(0f,0,0f), 2f)
        .SetEase(Ease.OutBack);
        ShowTitle();
        btnSelected = true;

        DOVirtual.DelayedCall(3.5f, ()=>{
            btnSelected = false;
            if(gameObject.GetComponent<Rigidbody2D>() != null) {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                HideTitle();
                if(!useForce) gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
                DOVirtual.Float(0f,1f,0.75f,GravityUpdate).SetEase(Ease.InQuart);
            }
            if(myUIFX != null) {
                myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
            }
        });

    }

    public void Off(){
        DOTween.Kill(gameObject.transform);
        if(gameObject.GetComponent<Rigidbody2D>() != null) {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
            HideTitle();
            btnSelected = false;
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        else if(myUIFX != null) {
            myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
        }
    }

    public void ReturnToOriginalPos() {
        DOTween.Kill(gameObject.transform);
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

        gameObject.transform.DOMove(initialPos, 0.75f).SetEase(Ease.OutCubic);
        gameObject.transform.DORotate(initialRotation, 0.5f).SetEase(Ease.OutCubic)
            .OnComplete(()=>{Off();});
    }
}
