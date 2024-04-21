// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lofelt.NiceVibrations
{
    public class PowerBarElement : MonoBehaviour
    {
        public float BumpDuration = 0.15f;
        public Color NormalColor;
        public Color InactiveColor;
        public AnimationCurve Curve;
        protected bool _active;
        protected bool _activeLastFrame;
        protected float _bumpDuration;

        protected Image _image;

        protected virtual void Awake()
        {
            _image = gameObject.GetComponent<Image>();
        }

        protected virtual void Update()
        {
            if (_active && !_activeLastFrame) StartCoroutine(ColorBump());
            _activeLastFrame = _active;
        }

        public virtual void SetActive(bool status)
        {
            _active = status;
            _image.color = status ? NormalColor : InactiveColor;
        }

        protected virtual IEnumerator ColorBump()
        {
            _bumpDuration = 0f;
            while (_bumpDuration < BumpDuration)
            {
                var curveValue = Curve.Evaluate(_bumpDuration / BumpDuration);
                _image.color = Color.Lerp(NormalColor, Color.white, curveValue);

                _bumpDuration += Time.deltaTime;
                yield return null;
            }

            _image.color = NormalColor;
        }
    }
}