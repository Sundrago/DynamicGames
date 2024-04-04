using UnityEngine;

namespace DynamicGames.System
{
    /// <summary>
    /// Resize the capsule collider attached to the game object.
    /// </summary>
    public class ResizeCapsuleCollider : MonoBehaviour
    {
        [Header("Game Components")] 
        [SerializeField] private CapsuleCollider2D collider2D;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private bool fixedWidth;

        private void Update()
        {
            if (Time.frameCount % 60 != 0) return;

            var rect = rectTransform.rect;
            if (collider2D != null)
                collider2D.size = new Vector2(fixedWidth ? collider2D.size.x : rect.width, rect.height);
            if (boxCollider2D != null)
                boxCollider2D.size = new Vector2(fixedWidth ? boxCollider2D.size.x : rect.width, rect.height);
        }
    }
}