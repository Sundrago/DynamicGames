// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class BallDemoBall : MonoBehaviour
    {
        public bool HapticsEnabled = true;
        public ParticleSystem HitParticles;
        public ParticleSystem HitPusherParticles;
        public LayerMask WallMask;
        public LayerMask PusherMask;
        public MMUIShaker LogoShaker;
        public AudioSource EmphasisAudioSource;
        protected Animator _ballAnimator;
        protected int _hitAnimationParameter;
        protected float _lastRaycastTimestamp;

        protected Rigidbody2D _rigidBody;

        protected virtual void Awake()
        {
            _rigidBody = gameObject.GetComponent<Rigidbody2D>();
            _ballAnimator = gameObject.GetComponent<Animator>();
            _hitAnimationParameter = Animator.StringToHash("Hit");
        }

        protected virtual void Update()
        {
            var raycastLength = 5f;

            Debug.DrawLine(transform.position, Vector3.down * raycastLength, Color.red);

            if (Time.time - _lastRaycastTimestamp > 1f)
            {
                _lastRaycastTimestamp = Time.time;
                var hit = Physics2D.Raycast(transform.position, Vector2.down, raycastLength, WallMask);
                if (hit.collider != null) HitBottom();
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (WallMask == (WallMask | (1 << collision.gameObject.layer))) HitWall();
        }

        protected virtual void HitBottom()
        {
            _rigidBody.AddForce(Vector2.up * 2500f);
            StartCoroutine(LogoShaker.Shake(0.2f));
        }

        protected virtual void HitWall()
        {
            var amplitude = _rigidBody.velocity.magnitude / 100f;
            HapticPatterns.PlayEmphasis(amplitude, 0.7f);
            EmphasisAudioSource.volume = amplitude;
            StartCoroutine(LogoShaker.Shake(0.2f));
            EmphasisAudioSource.Play();
            _ballAnimator.SetTrigger(_hitAnimationParameter);
        }

        public virtual void HitPusher()
        {
            HitPusherParticles.Play();
            HapticController.fallbackPreset = HapticPatterns.PresetType.Selection;
            HapticPatterns.PlayEmphasis(0.85f, 0.05f);
            EmphasisAudioSource.volume = 0.1f;
            StartCoroutine(LogoShaker.Shake(0.2f));
            EmphasisAudioSource.Play();
            _ballAnimator.SetTrigger(_hitAnimationParameter);
        }
    }
}