// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class WobbleButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [Header("Bindings")] public Camera TargetCamera;

        public AudioSource SpringAudioSource;
        public Animator TargetAnimator;

        [Header("Haptics")] public HapticSource SpringHapticSource;

        [Header("Colors")] public Image TargetModel;

        [Header("Wobble")] public float OffDuration = 0.1f;

        public float MaxRange;
        public AnimationCurve WobbleCurve;
        public float DragResetDuration = 4f;
        public float WobbleFactor = 2f;
        protected Canvas _canvas;
        protected float _dragEndedAt;

        protected Vector3 _dragEndedPosition;
        protected bool _draggedOnce;
        protected bool _dragging;
        protected Vector3 _dragResetDirection;
        protected Vector3 _eventPosition;
        protected float _initialZPosition;

        protected Vector3 _neutralPosition;
        protected Vector3 _newTargetPosition;
        protected PointerEventData _pointerEventData;
        protected int _pointerID;
        protected bool _pointerOn;
        protected RectTransform _rectTransform;
        protected int _sparkAnimationParameter;
        protected int[] _wobbleAndroidAmplitude = { 0, 40, 0, 80 };

        protected long[] _wobbleAndroidPattern = { 0, 40, 40, 80 };
        protected Vector2 _workPosition;
        public RenderMode ParentCanvasRenderMode { get; protected set; }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (_pointerOn && !_dragging)
            {
                _newTargetPosition = GetWorldPosition(_pointerEventData.position);

                var distance = (_newTargetPosition - _neutralPosition).magnitude;

                if (distance < MaxRange)
                    _dragging = true;
                else
                    _dragging = false;
            }

            if (_dragging)
                StickToPointer();
            else
                GoBackToInitialPosition();
        }

        public virtual void OnPointerEnter(PointerEventData data)
        {
            _pointerID = data.pointerId;
            _pointerEventData = data;
            _pointerOn = true;
        }

        public virtual void OnPointerExit(PointerEventData data)
        {
            _eventPosition = _pointerEventData.position;

            _newTargetPosition = GetWorldPosition(_eventPosition);
            _newTargetPosition = Vector2.ClampMagnitude(_newTargetPosition - _neutralPosition, MaxRange);
            _newTargetPosition = _neutralPosition + _newTargetPosition;
            _newTargetPosition.z = _initialZPosition;

            _dragging = false;
            _dragEndedPosition = _newTargetPosition;
            _dragEndedAt = Time.time;
            _dragResetDirection = _dragEndedPosition - _neutralPosition;
            _pointerOn = false;

            TargetAnimator.SetTrigger(_sparkAnimationParameter);
            SpringAudioSource.Play();
            SpringHapticSource.Play();
        }

        public virtual void SetPitch(float newPitch)
        {
            SpringAudioSource.pitch = newPitch;
            SpringHapticSource.frequencyShift = NiceVibrationsDemoHelpers.Remap(newPitch, 0.3f, 1f, -1.0f, 1.0f);
        }

        public virtual void Initialization()
        {
            _sparkAnimationParameter = Animator.StringToHash("Spark");
            ParentCanvasRenderMode = GetComponentInParent<Canvas>().renderMode;
            _canvas = GetComponentInParent<Canvas>();
            _initialZPosition = transform.position.z;
            _rectTransform = gameObject.GetComponent<RectTransform>();
            SetNeutralPosition();
        }

        public virtual void SetNeutralPosition()
        {
            _neutralPosition = _rectTransform.transform.position;
        }

        protected virtual Vector3 GetWorldPosition(Vector3 testPosition)
        {
            if (ParentCanvasRenderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
                    testPosition, _canvas.worldCamera, out _workPosition);
                return _canvas.transform.TransformPoint(_workPosition);
            }

            return testPosition;
        }

        protected virtual void StickToPointer()
        {
            _draggedOnce = true;
            _eventPosition = _pointerEventData.position;

            _newTargetPosition = GetWorldPosition(_eventPosition);

            // We clamp the stick's position to let it move only inside its defined max range
            _newTargetPosition = Vector2.ClampMagnitude(_newTargetPosition - _neutralPosition, MaxRange);
            _newTargetPosition = _neutralPosition + _newTargetPosition;
            _newTargetPosition.z = _initialZPosition;

            transform.position = _newTargetPosition;
        }

        protected virtual void GoBackToInitialPosition()
        {
            if (!_draggedOnce) return;

            if (Time.time - _dragEndedAt < DragResetDuration)
            {
                var time = Remap(Time.time - _dragEndedAt, 0f, DragResetDuration, 0f, 1f);
                var value = WobbleCurve.Evaluate(time) * WobbleFactor;
                _newTargetPosition = Vector3.LerpUnclamped(_neutralPosition, _dragEndedPosition, value);
                _newTargetPosition.z = _initialZPosition;
            }
            else
            {
                _newTargetPosition = _neutralPosition;
                _newTargetPosition.z = _initialZPosition;
            }

            transform.position = _newTargetPosition;
        }

        protected virtual float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}