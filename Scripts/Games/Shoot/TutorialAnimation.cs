using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Shoot
{
    public class TutorialAnimation : MonoBehaviour
    {
        [SerializeField] private float dist, speed;
        private Image img;
        private Vector3 startPosition;
        
        private void Start()
        {
            startPosition = gameObject.transform.position;
            img = GetComponent<Image>();
        }
        
        private void Update()
        {
            gameObject.transform.position = startPosition + new Vector3(Mathf.Sin(Time.time * speed) * dist,
                Mathf.Cos(Time.time * speed) * dist, 0);
        }

        public void Show()
        {
            var color = Color.white;
            color.a = 0;

            img.color = color;
            img.DOFade(1, 2f);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            img.DOFade(0, 0.5f)
                .OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}