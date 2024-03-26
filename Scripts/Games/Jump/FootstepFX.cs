using DG.Tweening;
using UnityEngine;

namespace Games.Jump
{
    /// <summary>
    ///     This class represents the visual effect when a player hits the floor by jumping.
    /// </summary>
    public class FootstepFX : MonoBehaviour
    {
        private void Start()
        {
            ScaleJumpHit();
            ChangeColorAndFade();
        }

        private void ScaleJumpHit()
        {
            gameObject.transform.DOScale(new Vector3(10f, 0.01f, 10f), 1.5f)
                .SetEase(Ease.OutQuint);
        }

        private void ChangeColorAndFade()
        {
            var mat = gameObject.GetComponent<MeshRenderer>().material;
            var color = mat.color;

            color.a = 0.7f;
            mat.DOColor(color, 0.5f);

            color.a = 0.01f;
            mat.DOColor(color, 1f)
                .SetDelay(0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { Destroy(gameObject); });
        }
    }
}