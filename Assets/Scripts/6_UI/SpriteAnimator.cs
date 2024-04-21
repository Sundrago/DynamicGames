using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// Controls the animation of a sprite with a sequence of sprites.
    /// </summary>
    public class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] public Sprite[] sprites;
        [SerializeField] public float interval = 1f;
        
        public bool isPlaying = true;
        public int pauseAtIdx = -1;
        private int idx;

        private Image image;
        private bool pauseAnimation;
        private SpriteRenderer spriteRenderer;

        private void OnEnable()
        {
            pauseAtIdx = -1;
            isPlaying = true;
            if (GetComponent<Image>() != null)
                image = GetComponent<Image>();
            if (GetComponent<SpriteRenderer>() != null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            StartCoroutine(SpriteAnim());
        }

        private IEnumerator SpriteAnim()
        {
            while (isPlaying)
            {
                yield return new WaitForSeconds(interval);

                if (!pauseAnimation) idx += 1;
                if (idx >= sprites.Length) idx = 0;

                if (image != null)
                    image.sprite = sprites[idx];
                if (spriteRenderer != null)
                    spriteRenderer.sprite = sprites[idx];

                if (pauseAtIdx == idx) isPlaying = false;
            }
        }

        public void RestartWithNoLoop()
        {
            idx = 0;
            pauseAtIdx = sprites.Length - 1;
            if (isPlaying == false)
            {
                isPlaying = true;
                StartCoroutine(SpriteAnim());
            }
        }

        public void PauseAnim()
        {
            pauseAnimation = true;
        }

        public void ResumeAnim()
        {
            pauseAnimation = false;
        }
    }
}