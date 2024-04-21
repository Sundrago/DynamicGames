using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// Responsible for animating the background sky.
    /// </summary>
    public class BGSkyAnimator : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed;
        [SerializeField] private bool initSelf;
        private GameObject childSkyObject;
        private RectTransform rectTransform;

        private void Start()
        {
            if (initSelf) InitializeSkyAnimator(GetComponent<Image>().sprite, scrollSpeed, 0f);
        }

        private void Update()
        {
            ScrollSkyObject();
        }

        public void InitializeSkyAnimator(Sprite sprite, float scrollSpeed, float transitionTime)
        {
            this.scrollSpeed = scrollSpeed;
            rectTransform = GetComponent<RectTransform>();

            if (!initSelf) rectTransform.sizeDelta = new Vector2(Screen.height * 1.78f, Screen.height);
            GetComponent<Image>().sprite = sprite;

            CreateChildSkyObject();
            FadeInSkyObject(transitionTime);
        }

        private void CreateChildSkyObject()
        {
            childSkyObject = Instantiate(gameObject, gameObject.transform);
            Destroy(childSkyObject.GetComponent<BGSkyAnimator>());
            childSkyObject.transform.position = gameObject.transform.position;

            var childPos = childSkyObject.GetComponent<RectTransform>().anchoredPosition;
            childPos.x -= rectTransform.sizeDelta.x;
            childSkyObject.GetComponent<RectTransform>().anchoredPosition = childPos;
        }

        private void FadeInSkyObject(float transitionTime)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
            childSkyObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GetComponent<Image>().DOFade(1, transitionTime);
            childSkyObject.GetComponent<Image>().DOFade(1, transitionTime);
        }

        private void ScrollSkyObject()
        {
            rectTransform.anchoredPosition = new Vector2(
                rectTransform.anchoredPosition.x + scrollSpeed * Time.deltaTime,
                rectTransform.anchoredPosition.y
            );

            LoopSkyScrolling();
        }

        private void LoopSkyScrolling()
        {
            if (rectTransform.anchoredPosition.x > rectTransform.sizeDelta.x)
                rectTransform.anchoredPosition = new Vector2(
                    rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x,
                    rectTransform.anchoredPosition.y
                );
            else if (scrollSpeed < 0 && rectTransform.anchoredPosition.x < 0)
                rectTransform.anchoredPosition = new Vector2(
                    rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x * 2f,
                    rectTransform.anchoredPosition.y
                );
        }
    }
}