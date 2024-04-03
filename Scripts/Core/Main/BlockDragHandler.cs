using System.Collections.Generic;
using Core.Pet;
using Core.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Core.Main
{
    /// <summary>
    /// Handles the dragging and interaction behavior of a block.
    /// </summary>s
    public class BlockDragHandler : MonoBehaviour, IDraggable
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private Ranking_UI ranking_UI;

        [Header("Game Components")] 
        [SerializeField] private TextMeshPro[] title;
        [SerializeField] private GameObject ShineEffects;
        [SerializeField] private GameObject myUIFX;
        [SerializeField] private BlockStatusManager.BlockType blockType;
        
        [Header("Player Interactions")] 
        public bool isButtonSelected;
        public bool hold;
        public GameObject miniisland;

        public Vector3 initialPos, initialRotation;
        private bool isDragging, showTitle, started;
        private float mouseDownTime, lastShake;
        private Vector2 dragStartPosition, dragDeltaPosition;
        private Color transparentColor;
        
        private SquareBlockCtrl squareBlockCtrl;
        private Rigidbody2D rigidbody2D;
        private MainCanvas mainCanvas;
        
        [Header("Constants")] 
        private const float HideTitleDelay = 0.5f;
        private const float ShakeThreshold = 30f;
        private const int ShakeInterval = 4;


        public void Start()
        {
            if (started) return;
            squareBlockCtrl = GetComponent<SquareBlockCtrl>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            mainCanvas = MainCanvas.Instance;
            InitPosition();
            InitColor();
            started = true;
        }

        private void InitPosition()
        {
            initialPos = gameObject.transform.position;
            initialRotation = gameObject.transform.eulerAngles;
        }

        private void InitColor()
        {
            transparentColor = new Color(1, 1, 1, 0);
            if (title.Length > 0)
            {
                foreach (var text in title)
                {
                    text.color = transparentColor;
                    text.gameObject.SetActive(false);
                }
            }
        }

        public void InstantiateEnergyFX()
        {
            if (ShineEffects != null)
            {
                if (myUIFX != null) myUIFX.GetComponent<ShineFxController>().DestroyFX();

                myUIFX = Instantiate(ShineEffects, ShineEffects.transform.parent.transform);
                myUIFX.GetComponent<ShineFxController>().InitiateFX(gameObject);
            }
        }


        private void OnMouseDown()
        {
            mainCanvas.Offall(gameObject);
            if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);

            dragStartPosition = Input.mousePosition;
            dragDeltaPosition = dragStartPosition;
            StartDragging();
            InstantiateEnergyFX();
        }
        
        private void StartDragging()
        {
            dragStartPosition = Input.mousePosition;
            dragDeltaPosition = dragStartPosition;
            
            isDragging = true;
            mouseDownTime = Time.time;

            rigidbody2D.isKinematic = true;
            rigidbody2D.angularVelocity = 0f;
        }
        
        private void OnMouseDrag()
        {
            if (!isDragging) return;

            Vector2 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPosition = Vector3.Lerp(transform.position, currentMousePosition, 0.3f);
            targetPosition.z = initialPos.z;
            transform.position = targetPosition;

            float delta = Vector2.Distance(dragDeltaPosition, Input.mousePosition);
            dragDeltaPosition = Input.mousePosition;
            if (delta > ShakeThreshold) OnShake();
        }

        private void OnMouseUp()
        {
            ResetRigidbody();
            UnselectButtonIfLongTimeElapsed();
            HandleClick();
            
            if (squareBlockCtrl != null) squareBlockCtrl.PunchLock();
            isDragging = false;
        }
        
        private void ResetRigidbody()
        {
            rigidbody2D.velocity = Vector3.zero;
            rigidbody2D.angularVelocity = 0f;
            HideTitle();
            rigidbody2D.isKinematic = false;
        }
        
        private void UnselectButtonIfLongTimeElapsed()
        {
            if (Time.time - mouseDownTime <= 1f) return;

            if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
            DOVirtual.Float(0f, 1f, 0.5f, UpdateGravity).SetEase(Ease.InQuart);
            isButtonSelected = false;
        }

        private void HandleClick()
        {
            var selected = false;
            if (isDragging & (Vector2.Distance(dragStartPosition, Input.mousePosition) < 20f)){
                if (Time.time - mouseDownTime < 0.4f)
                {
                    selected = true;
                    OnButtonClicked();
                    AnimateAndKillFX(30f);
                }
            }
            if (!selected)
            {
                isButtonSelected = false;        
                AnimateAndKillFX(3f);

            }
        }
        
        private void AnimateAndKillFX(float duration)
        {
            transform.DOScale(gameObject.transform.localScale, duration).OnComplete(KillFX);
        }

        private void UpdateGravity(float gravity)
        {
            rigidbody2D.gravityScale = gravity;
        }

        private void ShowTitle(bool forceRightSide = false)
        {
            if (showTitle) return;
            if (squareBlockCtrl != null) squareBlockCtrl.ShowLock();
            if (title.Length <= 0) return;

            foreach (var titleText in title)
            {
                titleText.gameObject.SetActive(true);
                if (DOTween.IsTweening(titleText)) DOTween.Kill(titleText);
                
                titleText.DOFade(1, 0.5f)
                    .SetEase(Ease.OutQuart);

                AdjustTextAlignment(titleText, forceRightSide);
            }

            showTitle = true;
        }

        private void AdjustTextAlignment(TextMeshPro titleText, bool forceRightSide)
        {
            if (forceRightSide || gameObject.transform.position.x <= 0)
            {
                titleText.alignment = TextAlignmentOptions.MidlineLeft;
                titleText.transform.localPosition = new Vector3(16.5f, titleText.transform.localPosition.y,
                    titleText.transform.localPosition.z);
            }
            else
            {
                titleText.alignment = TextAlignmentOptions.MidlineRight;
                titleText.transform.localPosition = new Vector3(-16.5f, titleText.transform.localPosition.y,
                    titleText.transform.localPosition.z);
            }
        }

        private void HideTitle()
        {
            if (squareBlockCtrl != null) squareBlockCtrl.HideLock();
            showTitle = false;
            
            foreach (var text in title)
            {
                if (DOTween.IsTweening(text)) DOTween.Kill(text);
                
                text.DOFade(0, 0.5f)
                    .SetEase(Ease.InQuart)
                    .OnComplete(() => { text.gameObject.SetActive(false); });
            }
            if (title.Length <= 0) return;
        }

        public void OnButtonClicked()
        {
            InstantiateEnergyFX();
            if(!IsBlockClicked()) return;
            HandleBlockAnimation();
            DeselectAfterTime();
            ShowTitle(true);
            isButtonSelected = true;
        }

        private bool IsBlockClicked()
        {
            if (!isButtonSelected) return true;
            
            if (squareBlockCtrl != null && squareBlockCtrl.isLocked && !squareBlockCtrl.isNotGame)
            {
                UnlockBtnManager.Instance.Init(this);
                AudioManager.Instance.PlaySfxByTag(SfxTag.Unable);
                TutorialManager.Instancee.DragSpriteBtnClicked();
                isButtonSelected = false;
                return true;
            }
            else
            {
                mainCanvas.GotoGame(blockType, gameObject);
                return false;
            }
        }

        private void HandleBlockAnimation()
        {
            if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
            rigidbody2D.isKinematic = true;
            gameObject.transform.DOMove(new Vector3(-0.4f, 0.6f, gameObject.transform.position.z), 2f)
                .SetEase(Ease.OutBack);
            gameObject.transform.DORotate(new Vector3(0f, 0, 0f), 2f)
                .SetEase(Ease.OutBack);
        }

        private void DeselectAfterTime()
        {
            gameObject.transform.DOScale(gameObject.transform.localScale, 10f)
                .OnComplete(() =>
                {
                    if (rigidbody2D != null)
                    {
                        if (hold) return;
                        rigidbody2D.velocity = Vector3.zero;
                        rigidbody2D.angularVelocity = 0f;
                        HideTitle();
                        rigidbody2D.isKinematic = false;
                        if (DOTween.IsTweening(gameObject.transform)) DOTween.Kill(gameObject.transform);
                        DOVirtual.Float(0f, 1f, 0.75f, UpdateGravity).SetEase(Ease.InQuart);
                    }
        
                    KillFX();
                    isButtonSelected = false;
                    UnlockBtnManager.Instance.Hide();
                });
        }

        public void Deactivate()
        {
            if (hold) return;
            MakeRigidbodyDynamic();
            HideTitle();
            KillFX();
            UnlockBtnManager.Instance.Hide();
            isButtonSelected = false;
        }

        private void MakeRigidbodyDynamic()
        {
            DOTween.Kill(gameObject.transform);
            if (rigidbody2D.isKinematic)
            {
                rigidbody2D.velocity = Vector3.zero;
                rigidbody2D.angularVelocity = 0f;
                rigidbody2D.gravityScale = 1f;
                rigidbody2D.isKinematic = false;
            }
        }

        public void ReturnToOriginalPosition()
        {
            DOTween.Kill(gameObject.transform);
            rigidbody2D.isKinematic = true;

            gameObject.transform.DOMove(initialPos, 0.75f).SetEase(Ease.OutCubic);
            gameObject.transform.DORotate(initialRotation, 0.5f).SetEase(Ease.OutCubic)
                .OnComplete(() => { Deactivate(); });
        }

        private void Update()
        {
            UpdateDragPosition();
            BacktoOriginalPosition();
        }

        private void UpdateDragPosition()
        {
            if (isDragging && Time.time - mouseDownTime > 0.3f)
            {
                ShowTitle();
                var targetAngle = transform.eulerAngles.z % 360;
                if (targetAngle < 180)
                    targetAngle = transform.eulerAngles.z - transform.eulerAngles.z % 360;
                else targetAngle = transform.eulerAngles.z + (360 - transform.eulerAngles.z % 360);
                
                transform.eulerAngles = Vector3.Lerp(gameObject.transform.localEulerAngles,
                    new Vector3(0, 0, targetAngle), 0.1f);
            }
        }

        private void BacktoOriginalPosition()
        {
            if (Time.frameCount % 60 == 0 && gameObject.transform.localPosition.y < -3000)
            {
                rigidbody2D.velocity = Vector2.zero;
                gameObject.transform.localPosition = Vector3.zero;
            }
        }

        public void Unlock()
        {
            if (squareBlockCtrl != null)
                squareBlockCtrl.UnLock();
        }


        private void OnShake()
        {
            if (Time.time < lastShake + ShakeInterval) return;
            lastShake = Time.time;

            var pets = GetActivePets();
            ShakePets(pets);
        }
        
        private List<Pet.PetController> GetActivePets()
        {
            var activePets = new List<Pet.PetController>();
            foreach (var petData in PetManager.Instance.GetActivePetConfigs())
            {
                bool isActive = petData.obj.activeSelf;
                bool isOnCurrentCorner = petData.obj.GetComponent<SurfaceMovement2D>().currentLocation.obj == gameObject;
                if (isActive && isOnCurrentCorner)
                {
                    Pet.PetController petController = petData.obj.GetComponent<Pet.PetController>();
                    activePets.Add(petController);
                }
            }
            return activePets;
        }

        private void ShakePets(List<Pet.PetController> pets)
        {
            float threshold = pets.Count < 3 ? 1 : 2.5f / pets.Count;
            foreach (var pet in pets)
            {
                if (Random.Range(0f, 1f) < threshold)
                {
                    pet.OnShake();
                }
            }
        }
        
        public void KillFX()
        {
            if (myUIFX != null) myUIFX.GetComponent<ShineFxController>().DestroyFX();
        }
    }
}