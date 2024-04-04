using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Pet;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.MainPage
{
    /// <summary>
    ///     Handles the dragging and interaction behavior of a title object.
    /// </summary>
    public class TitleDragHandler : MonoBehaviour, IDraggable
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float tolerance;
        [SerializeField] private Rigidbody2D rigidbody;
        
        private bool isDrag;
        private Vector2 startMousePosition, startObjectPosition;
        private Vector3 title_initPos, title_initRotation;

        private void Start()
        {
            title_initPos = gameObject.transform.position;
            title_initRotation = gameObject.transform.eulerAngles;
        }

        private void LateUpdate()
        {
            if (!isDrag) return;

            if (Vector2.Distance(startMousePosition, Input.mousePosition) > tolerance)
            {
                OnMouseUp();
                return;
            }

            var mouseOffset = startMousePosition - (Vector2)Input.mousePosition;
            rectTransform.anchoredPosition = startObjectPosition - mouseOffset;
        }

        public void OnMouseDown()
        {
            startMousePosition = Input.mousePosition;
            startObjectPosition = rectTransform.anchoredPosition;
            isDrag = true;
        }

        private void OnMouseUp()
        {
            isDrag = false;

            var petsOnTitle = GetPetsOnTitle();
            for (var i = 0; i < petsOnTitle.Count; i++)
            {
                var thres = petsOnTitle.Count < 3 ? 1 : 3f / petsOnTitle.Count;
                if (Random.Range(0f, 1f) < thres) petsOnTitle[i].OnShake();
            }
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

        private List<PetObject> GetPetsOnTitle()
        {
            var petsOnTitle = new List<PetObject>();
            foreach (var petdata in PetManager.Instance.GetActivePetConfigs())
                if (petdata.obj.activeSelf && petdata.obj.GetComponent<PetSurfaceMovement2D>().currentPlace ==
                    PetSurfaceMovement2D.CurrentLocationType.Title)
                    petsOnTitle.Add(petdata.component);
            return petsOnTitle;
        }
    }
}