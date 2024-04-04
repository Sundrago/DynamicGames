using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Represents a tutorial animation for a shooting mini-game.
    /// </summary>
    public class TutorialAnimation : MonoBehaviour
    {
        private const float Dist = 0.05f;
        private const float Speed = 4f;

        private Image img;
        private Vector3 startPosition;

        private void Start()
        {
            startPosition = gameObject.transform.position;
            img = GetComponent<Image>();
        }

        private void Update()
        {
            gameObject.transform.position = startPosition + new Vector3(Mathf.Sin(Time.time * Speed) * Dist,
                Mathf.Cos(Time.time * Speed) * Dist, 0);
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