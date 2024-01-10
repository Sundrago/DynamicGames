using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;

public class DragSprite : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TextMeshPro[] title;
    [SerializeField] GameObject UIFX;
    [SerializeField] GameObject myUIFX;
    [SerializeField] MainCanvas main;
    [SerializeField]
    private Ranking_UI ranking_UI;
    
    [SerializeField] private BlockStatusManager.BlockType blockType;

    public bool btnSelected = false;
    public GameObject miniisland;

    private Vector2 startPosition;
    private Color transparent = Color.white;
    public Vector3 initialPos, initialRotation;

    private bool drag = false;
    private bool showtitle = false;
    private float mouseDownTime;
    private SquareBlockCtrl squareBlockCtrl = null;

    private bool started = false;
    private void Awake()
    {
        
    }

    public void Start()
    {
        if(started) return;
        initialPos = gameObject.transform.position;
        initialPos = gameObject.transform.position;
        initialRotation = gameObject.transform.eulerAngles;
        squareBlockCtrl = GetComponent<SquareBlockCtrl>();
        transparent.a = 0;
        if(title.Length > 0) {
            foreach(TextMeshPro text in title) {
                text.color = transparent;
                text.gameObject.SetActive(false);
            }
        }

        started = true;
    }

    public void OnPointerDown(PointerEventData data)
    {
        main.Offall(gameObject);
        if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

        startPosition = data.position;
        
        drag = true;
        mouseDownTime = Time.time;
        
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        InstantiateEnergyFX();
    }

    public void InstantiateEnergyFX()
    {
        if(UIFX != null) {
            if(myUIFX != null) {
                myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
            }
            myUIFX = Instantiate(UIFX, UIFX.transform.parent.transform);
            myUIFX.GetComponent<EnergyUIFXCtrl>().InitiateFX(gameObject);
        }
    }
    
    public void OnDrag(PointerEventData data)
    {
        if(!drag) return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(data.position);
        Vector3 targetPos = Vector3.Lerp(this.transform.position, mousePosition, 0.3f);
        targetPos.z = initialPos.z;
        this.transform.position = targetPos; 
    }
    
    public void OnPointerUp(PointerEventData data)
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        HideTitle();
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;

        bool selected = false;
        
        if(Time.time - mouseDownTime > 1f) {
            if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
            DOVirtual.Float(0f,1f,0.5f,GravityUpdate).SetEase(Ease.InQuart);
            btnSelected = false;
        }
        
        if (drag & (Vector2.Distance(startPosition, data.position) < 20f))
        {
            if (Time.time - mouseDownTime < 0.4f)
            {
                selected = true;
                BtnClicked();
                transform.DOScale(gameObject.transform.localScale, 30f).OnComplete(()=> {
                    if (myUIFX != null)
                    {
                        myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
                    }
                });
            }
        }

        if(!selected) {
            btnSelected = false;
            transform.DOScale(gameObject.transform.localScale, 3f).OnComplete( ()=> {
                if (myUIFX != null)
                {
                    myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
                }
            });
            
        }
        
        if(squareBlockCtrl!=null) squareBlockCtrl.PunchLock();
        drag = false;
    }
    
   public void OnMouseDown()
    {
        // if (EventSystem.current.IsPointerOverGameObject()) return;
        //
        // main.Offall(gameObject);
        // if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        //
        // PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        // eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        // startPosition = eventDataCurrentPosition.position;
        //
        // drag = true;
        // mouseDownTime = Time.time;
        //
        // gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        // gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        //
        // if(UIFX != null) {
        //     if(myUIFX != null) {
        //         myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
        //     }
        //     myUIFX = Instantiate(UIFX, UIFX.transform.parent.transform);
        //     myUIFX.GetComponent<EnergyUIFXCtrl>().InitiateFX(gameObject);
        // }
    }

    void OnMouseDrag()
    {
        // if (drag)
        // {
        //     Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector3 targetPos = Vector3.Lerp(this.transform.position, mousePosition, 0.3f);
        //     targetPos.z = initialPos.z;
        //     this.transform.position = targetPos; 
        //     if(Time.time - mouseDownTime > 0.3f) {
        //
        //         ShowTitle();
        //
        //         float targetAngle = this.transform.eulerAngles.z % 360;
        //         if(targetAngle < 180) targetAngle = this.transform.eulerAngles.z - this.transform.eulerAngles.z % 360;
        //         else targetAngle = this.transform.eulerAngles.z + (360 - this.transform.eulerAngles.z % 360);
        //         this.transform.eulerAngles = Vector3.Lerp(gameObject.transform.localEulerAngles, new Vector3(0,0,targetAngle), 0.1f);
        //     }
        // }
    }

    private void OnMouseUp()
    {
        // gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        // gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        // HideTitle();
        // gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        // if(Time.time - mouseDownTime > 1f) {
        //     if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
        //     DOVirtual.Float(0f,1f,0.5f,GravityUpdate).SetEase(Ease.InQuart);
        // }
        //
        // if (drag & Vector2.Distance(startPosition, new Vector2(Input.mousePosition.x, Input.mousePosition.y)) < 15f)
        // {
        //     if (Time.time - mouseDownTime < 0.4f && EventSystem.current.IsPointerOverGameObject() == false)
        //         BtnClicked();
        // }
        //
        // DOVirtual.DelayedCall(3f, () => {
        //     if (myUIFX != null)
        //     {
        //         myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
        //     }
        // });
        // drag = false;
    }

    void GravityUpdate(float gravity) {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = gravity;
    }

    void ShowTitle(bool forceRightSide = false){
        if(showtitle) return;
        if(squareBlockCtrl!=null) squareBlockCtrl.ShowLock();
        if(title.Length <= 0) return;
        // transparent.a = 0f;
        foreach(TextMeshPro text in title) {
            // text.color = transparent;
            text.gameObject.SetActive(true);
            if(DOTween.IsTweening(text)) DOTween.Kill(text);
            text.DOFade(1, 0.5f)
            .SetEase(Ease.OutQuart);
            
            //setposition;

            if (forceRightSide || gameObject.transform.position.x <= 0)
            {
                text.alignment = TextAlignmentOptions.MidlineLeft;
                text.transform.localPosition = new Vector3(16.5f, text.transform.localPosition.y, text.transform.localPosition.z);
            }
            else
            {
                text.alignment = TextAlignmentOptions.MidlineRight;
                text.transform.localPosition = new Vector3(-16.5f, text.transform.localPosition.y, text.transform.localPosition.z);
            } 
        }
        showtitle = true;
    }

    void HideTitle(){
        if(squareBlockCtrl!=null) squareBlockCtrl.HideLock();
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

    public void BtnClicked() {
        print("BtnClicked");
        
        if(btnSelected) {
            if (squareBlockCtrl != null && squareBlockCtrl.isLocked == true && !squareBlockCtrl.isNotGame)
            {
                UnlockBtnManager.Instance.Init(this);
                AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.unable);
                TutorialManager.Instancee.tutorialB_Check();
                btnSelected = false;
            }
            else
            {
                main.GotoGame(blockType, gameObject);
                return;
            }
        }

        if (true) //(miniisland != null)
        {
            if(DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
            gameObject.transform.DOMove(new Vector3(-0.4f,0.9f,gameObject.transform.position.z), 2f)
            .SetEase(Ease.OutBack);
            gameObject.transform.DORotate(new Vector3(0f,0,0f), 2f)
            .SetEase(Ease.OutBack);
            
            gameObject.transform.DOScale(gameObject.transform.localScale, 10f)
                .OnComplete(() => {
                    if (gameObject.GetComponent<Rigidbody2D>() != null)
                    {
                        gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                        gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
                        HideTitle();
                        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
                        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
                        DOVirtual.Float(0f, 1f, 0.75f, GravityUpdate).SetEase(Ease.InQuart);
                    }
                    if (myUIFX != null)
                    {
                        myUIFX.GetComponent<EnergyUIFXCtrl>().DestroyFX();
                    }
                    btnSelected = false;
                    UnlockBtnManager.Instance.Hide();
                });
            ShowTitle(true);
        }
        else
        {
            ShowTitle(false);
            transform.DOScale(gameObject.transform.localScale, 3.5f).OnComplete(() => {
                btnSelected = false;
            });
        }
        btnSelected = true;
    }

    public void Off(){
        DOTween.Kill(gameObject.transform);
        if (gameObject.GetComponent<Rigidbody2D>().isKinematic)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody2D>().angularVelocity = 0f;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
            gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        UnlockBtnManager.Instance.Hide();
        HideTitle();
        btnSelected = false;
        if(myUIFX != null) {
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

    private void Update()
    {
        if (drag)
        {
            if(Time.time - mouseDownTime > 0.3f) {
                ShowTitle();
                float targetAngle = this.transform.eulerAngles.z % 360;
                if(targetAngle < 180) targetAngle = this.transform.eulerAngles.z - this.transform.eulerAngles.z % 360;
                else targetAngle = this.transform.eulerAngles.z + (360 - this.transform.eulerAngles.z % 360);
                this.transform.eulerAngles = Vector3.Lerp(gameObject.transform.localEulerAngles, new Vector3(0,0,targetAngle), 0.1f);
            }
        }
        
        if(Time.frameCount % 60 != 0) return;
        if (gameObject.transform.localPosition.y < -3000)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            gameObject.transform.localPosition = Vector3.zero;
        }
    }

    public void Unlock()
    {
        if (squareBlockCtrl != null)
            squareBlockCtrl.UnLock();
    }
}
