// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class HapticCurve : MonoBehaviour
    {
        [Range(0f, 1f)] public float Amplitude = 1f;

        [Range(0f, 1f)] public float Frequency;

        public int PointsCount = 50;
        public float AmplitudeFactor = 3;
        public RectTransform StartPoint;
        public RectTransform EndPoint;

        [Header("Movement")] public bool Move;

        public float MovementSpeed = 1f;
        protected Camera _camera;

        protected Canvas _canvas;
        protected Vector3 _endPosition;

        protected Vector3 _startPosition;

        protected LineRenderer _targetLineRenderer;
        protected Vector3 _workPoint;

        [Range(1f, 4f)] private float Period = 1;

        protected List<Vector3> Points;

        protected virtual void Awake()
        {
            Initialization();
        }

        protected virtual void Update()
        {
            UpdateCurve(Amplitude, Frequency);
        }

        protected virtual void Initialization()
        {
            Points = new List<Vector3>();
            _canvas = gameObject.GetComponentInParent<Canvas>();
            _targetLineRenderer = gameObject.GetComponent<LineRenderer>();
            _camera = _canvas.worldCamera;
            DrawCurve();
        }

        protected virtual void DrawCurve()
        {
            _startPosition = StartPoint.transform.position;
            _startPosition.z -= 0.1f;
            _endPosition = EndPoint.transform.position;
            _endPosition.z -= 0.1f;

            Points.Clear();

            for (var i = 0; i < PointsCount; i++)
            {
                var t = NiceVibrationsDemoHelpers.Remap(i, 0, PointsCount, 0f, 1f);
                var sinValue = MMSignal.GetValue(t, MMSignal.SignalType.Sine, 1f, AmplitudeFactor, Period, 0f);

                if (Move)
                    sinValue = MMSignal.GetValue(t + Time.time * MovementSpeed, MMSignal.SignalType.Sine, 1f,
                        AmplitudeFactor, Period, 0f);

                _workPoint.x = Mathf.Lerp(_startPosition.x, _endPosition.x, t);
                _workPoint.y = sinValue * Amplitude + _startPosition.y;
                _workPoint.z = _startPosition.z;
                Points.Add(_workPoint);
            }

            _targetLineRenderer.positionCount = PointsCount;
            _targetLineRenderer.SetPositions(Points.ToArray());
        }

        public virtual void UpdateCurve(float amplitude, float frequency)
        {
            Amplitude = amplitude;
            Frequency = frequency;
            Period = NiceVibrationsDemoHelpers.Remap(frequency, 0f, 1f, 1f, 4f);
            DrawCurve();
        }
    }
}