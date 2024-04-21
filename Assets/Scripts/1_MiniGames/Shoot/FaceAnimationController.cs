using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Controls the animations of a face in a shooting mini-game.
    /// </summary>
    public class FaceAnimationController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private RectTransform face;
        [SerializeField] private float offsetAmt;

        private Vector3 originalPos;

        private void Start()
        {
            face = GetComponent<RectTransform>();
            originalPos = face.anchoredPosition;
        }

        private void LateUpdate()
        {
            var vecNormal = (Vector2)gameObject.transform.position - (Vector2)player.position;
            face.anchoredPosition = originalPos + (Vector3)vecNormal.normalized * offsetAmt;
        }
    }
}