using System;
using System.Collections;
using System.Collections.Generic;
using Core.Pet;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Core.Main
{
    /// <summary>
    /// Handles dragging behavior for a title object.
    /// </summary>
    public class TitleDragHandler : MonoBehaviour, IDraggable
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float tolerance;
        [SerializeField] private Rigidbody2D rigidbody;
        private Vector3 title_initPos, title_initRotation;
        Vector2 startMousePosition, startObjectPosition;
        bool isDrag = false;

        private void Start()
        {
            title_initPos = gameObject.transform.position;
            title_initRotation = gameObject.transform.eulerAngles;
        }

        public void OnMouseDown()
        {
            startMousePosition = Input.mousePosition;
            startObjectPosition = rectTransform.anchoredPosition;
            isDrag = true;
        }

        private void LateUpdate()
        {
            if (!isDrag) return;

            if (Vector2.Distance(startMousePosition, Input.mousePosition) > tolerance)
            {
                OnMouseUp();
                return;
            }

            Vector2 mouseOffset = startMousePosition - (Vector2)Input.mousePosition;
            rectTransform.anchoredPosition = startObjectPosition - mouseOffset;
        }

        private void OnMouseUp()
        {
            isDrag = false;

            List<Pet.PetObject> petsOnTitle = GetPetsOnTitle();
            for (int i = 0; i < petsOnTitle.Count; i++)
            {
                float thres = petsOnTitle.Count < 3 ? 1 : 3f / petsOnTitle.Count;
                if (Random.Range(0f, 1f) < thres) petsOnTitle[i].OnShake();
            }
        }

        List<Pet.PetObject> GetPetsOnTitle()
        {
            List<Pet.PetObject> petsOnTitle = new List<Pet.PetObject>();
            foreach (var petdata in PetManager.Instance.GetActivePetConfigs())
            {
                if (petdata.obj.activeSelf && petdata.obj.GetComponent<SurfaceMovement2D>().currentPlace ==
                    SurfaceMovement2D.CurrentLocationType.Title)
                {
                    petsOnTitle.Add(petdata.component);
                }
            }
            return petsOnTitle;
        }

        public void ReturnToOriginalPosition()
        {
            rigidbody.isKinematic = true;
            DOTween.Kill(gameObject.transform);
            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

            gameObject.transform.DOMove(title_initPos, 0.75f).SetEase(Ease.OutCubic);
            gameObject.transform.DORotate(title_initRotation, 0.5f).SetEase(Ease.OutCubic)
                .OnComplete(() => { rigidbody.isKinematic = false; });
        }
    }
}