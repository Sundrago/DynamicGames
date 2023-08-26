using UnityEngine;

namespace EpicToonFX
{

    public class ETFXSpriteBouncer : MonoBehaviour
    {
        public float scaleAmount = 1.1f; // How much the sprite should scale up vertically
        public float scaleDuration = 1f; // How long it takes to complete a full scale cycle

        private Vector3 startScale; // Original scale of the sprite
        private float scaleTimer = 0f; // Timer for scaling animation

        private void Start()
        {
            startScale = transform.localScale; // Store the original scale of the sprite

            // If the sprite is already scaled, adjust the startScale accordingly
            if (startScale.y != 1f)
            {
                float adjustedScale = startScale.y / scaleAmount;
                startScale = new Vector3(startScale.x, adjustedScale, startScale.z);
            }
        }

        private void Update()
        {
            scaleTimer += Time.deltaTime; // Update the timer

            float t = Mathf.Clamp01(scaleTimer / scaleDuration); // Calculate the interpolation factor with a clamp

            float verticalScale = Mathf.Lerp(startScale.y, startScale.y * scaleAmount, t) + Mathf.PingPong(scaleTimer / scaleDuration, 0.1f);; // Interpolate the vertical scale
            Vector3 newScale = new Vector3(startScale.x, verticalScale, startScale.z); // Calculate the new scale

            transform.localScale = newScale; // Update the scale of the sprite
        }
    }
}