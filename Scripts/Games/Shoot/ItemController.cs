using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Shoot
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private Transform timerScalerUI;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public Vector3 RandomVector;
        public ItemType itemType { get; private set; }
        public float Velocity { get; private set; }

        private float duration;
        private float startTime;

        public void Init(Vector2 position, ItemType type, Sprite sprite, float velocity = 0.2f, float duration = 0)
        {
            this.duration = duration > 0 ? duration : Random.Range(6f, 11f);
            gameObject.transform.localPosition = position;
            RandomVector = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
            itemType = type;
            Velocity = velocity;
            spriteRenderer.sprite = sprite;
            startTime = Time.time;
            GetNormalizedTimer();
        }
        
        public float GetNormalizedTimer()
        {
            var normalTimer = 1f - Mathf.Clamp((Time.time - startTime) / duration, 0f, 1f);
            timerScalerUI.localScale = new Vector3(EaseInOutQuad(normalTimer), 1f, 1f);
            return normalTimer;
            float EaseInOutQuad(float x)
            {
                return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
            }
        }
    }
}