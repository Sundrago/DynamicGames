using DynamicGames.System;
using DynamicGames.UI;
using UnityEngine;

namespace DynamicGames.MiniGames.Jump
{
    /// <summary>
    ///     Controls the movement and behavior of a player in a 3D environment.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController3D : MonoBehaviour
    {
        [Header("Managers and Controllers")] [SerializeField]
        private GameManager jumpStage;

        [SerializeField] private SfxController sfxController;

        [Header("Game Components")] [SerializeField]
        private SpriteAnimator spriteAnimator;

        [SerializeField] private FootstepFX footstepFXPrefab;
        [SerializeField] private Transform footstepsHolder;

        [Header("Game Status")] [SerializeField]
        private float jumpForce;

        [SerializeField] private float posYAdd;
        [SerializeField] private float holdTime;

        private Rigidbody rigidbody;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Time.frameCount % 5 != 0) return;

            if (Mathf.Abs(rigidbody.velocity.y) >= 0.01f)
                holdTime = Time.time;

            if (Time.time - holdTime > 1f)
                rigidbody.AddForce(new Vector3(0, jumpForce, 0));
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsCollisionValid(other))
            {
                AdjustPositionAndForce(other);
                PlayAnimation();
                HandleFootIndex(other);
                sfxController.PlayWaterSoundEffect();
            }
        }

        private void OnCollisionStay(Collision other)
        {
            OnCollisionEnter(other);
        }

        private bool IsCollisionValid(Collision other)
        {
            return other.gameObject.CompareTag("footstep")
                   & (rigidbody.velocity.y <= 0)
                   & (gameObject.transform.position.y > other.transform.position.y);
        }

        private void AdjustPositionAndForce(Collision other)
        {
            var newPos = gameObject.transform.position;
            newPos.y = other.gameObject.transform.position.y + posYAdd;
            gameObject.transform.position = newPos;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(new Vector3(0, jumpForce, 0));
        }

        private void PlayAnimation()
        {
            spriteAnimator.RestartWithNoLoop();
        }

        private void HandleFootIndex(Collision other)
        {
            if (TryParseFootIndex(other.gameObject.transform.parent.gameObject.name, out var footIdx))
                if (footIdx > jumpStage.CurrentScore)
                    UpdateScoreAndFX(footIdx, other);
        }

        private bool TryParseFootIndex(string gameObjectName, out int footIdx)
        {
            return int.TryParse(gameObjectName, out footIdx);
        }

        private void UpdateScoreAndFX(int footIdx, Collision other)
        {
            jumpStage.CurrentScore = footIdx;
            jumpStage.UpdateScoreUI(footIdx);
            var obj = Instantiate(footstepFXPrefab, footstepsHolder.transform);
            obj.transform.localPosition = other.transform.parent.transform.localPosition;
            obj.gameObject.SetActive(true);
        }
    }
}