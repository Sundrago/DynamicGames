using System.Collections.Generic;
using Core.Pet;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class FruitsManager : MonoBehaviour
    {
        [SerializeField] private List<FruitController> fruitItems;
        [SerializeField] private Image obj_prefeb;

        [Button]
        public void InstantiateFruitObject(int idx, PetType type, float velocity, float duration)
        {
            var obj = Instantiate(obj_prefeb.gameObject, gameObject.transform);
            obj.transform.position = fruitItems[idx].fruitImage.transform.position;
            obj.GetComponent<Image>().sprite = fruitItems[idx].fruitImage.sprite;
            obj.SetActive(true);
            FruitAnimation(obj, PetManager.Instance.GetPetDataByType(type).obj.transform, velocity, duration);
        }

        public void FruitAnimation(GameObject obj, Transform endPosTransform, float _velocity = 100f,
            float durationFactor = 1f)
        {
            if (DOTween.IsTweening(obj.transform)) DOTween.Kill(obj.transform);

            var startPos = obj.transform.position;
            var path = new Vector3[3];
            var velocity = _velocity * Random.Range(0.8f, 1.2f);
            var angle = Random.Range(0f, 2f) * Mathf.PI;

            obj.transform.localScale = Vector3.one;
            obj.transform.position = startPos;

            path[2] = Camera.main.WorldToScreenPoint(endPosTransform.position);
            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
            path[0] = Vector3.Lerp(path[0], path[2], 0.2f);

            var diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - diff * Random.Range(0.3f, 0.8f);

            SetupFruitAnimation(obj, path, durationFactor, endPosTransform);
        }

        private void SetupFruitAnimation(GameObject obj, Vector3[] path, float durationFactor,
            Transform endPosTransform)
        {
            obj.transform.DOMove(path[0], 0.3f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    path[2] = Camera.main.WorldToScreenPoint(endPosTransform.position);
                    obj.transform.DOPath(path, 0.7f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase(Ease.InOutCubic)
                        .OnComplete(() => { Destroy(obj); });
                    obj.transform.DOScale(Vector3.zero, 0.65f * durationFactor)
                        .SetEase(Ease.InQuart);
                });
        }

        public void FruitBtnClicked(int idx)
        {
            // Spawn(idx, PetType.Foxy, 100f, 1f);
        }
    }
}