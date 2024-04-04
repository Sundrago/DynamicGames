using UnityEngine;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Moves the pet information panel to a specified position relative to selected pet.
    /// </summary>
    public class PetInfoPanelMover : MonoBehaviour
    {
        private const float lerpA = 0.1f;
        private const float lerpB = 0.6f;

        [Header("Game Components")] [SerializeField]
        private Camera camera;

        [SerializeField] public Transform targetPetPos, petSelectionIcon;
        [SerializeField] private Transform pointMarker, pointMarkerTop;
        [SerializeField] private Transform constraint_left, constraint_right;
        private float offsetY = 550f;

        private void Start()
        {
            offsetY = Screen.height / 7f + 150;
        }

        private void Update()
        {
            if (targetPetPos == null) return;

            var pos = camera.WorldToScreenPoint(targetPetPos.position);
            var panelTargetPos = new Vector3(gameObject.transform.position.x,
                pos.y + (targetPetPos.position.y < Screen.height / 2f ? -offsetY : +offsetY), 0);

            if (targetPetPos.position.y < 0)
            {
                panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y + offsetY, 0);
                pointMarkerTop.gameObject.SetActive(false);
                pointMarker.gameObject.SetActive(true);
                pointMarker.transform.position = Vector3.Lerp(pointMarker.transform.position,
                    new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x),
                        pointMarker.transform.position.y, 0), lerpB);
            }
            else
            {
                panelTargetPos = new Vector3(gameObject.transform.position.x, pos.y - offsetY * 1.2F, 0);
                pointMarkerTop.gameObject.SetActive(true);
                pointMarker.gameObject.SetActive(false);
                pointMarkerTop.transform.position = Vector3.Lerp(pointMarkerTop.transform.position,
                    new Vector3(Mathf.Clamp(pos.x, constraint_left.position.x, constraint_right.position.x),
                        pointMarkerTop.transform.position.y, 0), lerpB);
            }

            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, panelTargetPos, lerpA);
            petSelectionIcon.position = pos;
        }
    }
}