using System.Collections;
using UnityEngine;

namespace EpicToonFX
{
    public class ETFXMouseOrbit : MonoBehaviour
    {
        public Transform target;
        public float distance = 12.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;
        public float distanceMin = 8f;
        public float distanceMax = 15f;
        public float smoothTime = 2f;

        [HideInInspector] public bool isAutoRotating;
        [HideInInspector] public ETFXEffectController etfxEffectController;
        [HideInInspector] public ETFXEffectControllerPooled etfxEffectControllerPooled;
        private readonly float autoRotationSmoothing = 0.02f;
        private float maxVelocityX = 0.1f;
        private float rotationXAxis;
        private float rotationYAxis;
        private float velocityX;
        private float velocityY;

        private void Start()
        {
            var angles = transform.eulerAngles;
            rotationYAxis = angles.y;
            rotationXAxis = angles.x;

            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().freezeRotation = true;
        }

        private void Update()
        {
            if (target)
            {
                if (Input.GetMouseButton(1))
                {
                    velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                    velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;

                    if (isAutoRotating) StopAutoRotation();
                }

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 15, distanceMin, distanceMax);
            }
        }

        private void FixedUpdate()
        {
            if (target)
            {
                rotationYAxis += velocityX;
                rotationXAxis -= velocityY;
                rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
                var toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
                var rotation = toRotation;

                if (Physics.Linecast(target.position, transform.position, out var hit)) distance -= hit.distance;

                var negDistance = new Vector3(0.0f, 0.0f, -distance);
                var position = Vector3.Lerp(transform.position, rotation * negDistance + target.position, 0.6f);

                transform.rotation = rotation;
                transform.position = position;
                velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
                velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }

        public void InitializeAutoRotation()
        {
            isAutoRotating = true;

            StartCoroutine(AutoRotate());
        }

        public void SetAutoRotationSpeed(float rotationSpeed)
        {
            maxVelocityX = rotationSpeed;
        }

        private void StopAutoRotation()
        {
            if (etfxEffectController != null)
                etfxEffectController.autoRotation = false;

            if (etfxEffectControllerPooled != null)
                etfxEffectControllerPooled.autoRotation = false;

            isAutoRotating = false;
            StopAllCoroutines();
        }

        private IEnumerator AutoRotate()
        {
            var lerpSteps = 0;

            while (lerpSteps < 30)
            {
                velocityX = Mathf.Lerp(velocityX, maxVelocityX, autoRotationSmoothing);

                yield return new WaitForFixedUpdate();
            }

            while (isAutoRotating)
            {
                velocityX = maxVelocityX;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}