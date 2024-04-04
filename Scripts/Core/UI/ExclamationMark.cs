using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.UI
{
    public class ExclamationMark : MonoBehaviour
    {
        private const float OffsetX = 0;
        private const float OffsetY = 140;
        [SerializeField] private Camera camera;
        private Action callback;
        private bool initialized;
        private Transform target;

        private void Update()
        {
            if (!initialized)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.transform.position = target.position;
            gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x + OffsetX,
                gameObject.transform.localPosition.y + OffsetY, gameObject.transform.localPosition.z);
        }

        [Button]
        public void Init(Transform target, Action callback = null)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            var defaultSizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
            rect.sizeDelta = Vector2.zero;

            rect.DOSizeDelta(defaultSizeDelta, 1f);

            initialized = true;
            this.target = target;
            this.callback = callback;
            gameObject.SetActive(true);
        }

        public void Clicked()
        {
            callback.Invoke();
            Destroy(gameObject);
            // gameObject.SetActive(false);
        }
    }
}