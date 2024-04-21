// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class BallDemoWall : MonoBehaviour
    {
        protected BoxCollider2D _boxCollider2D;
        protected RectTransform _rectTransform;

        protected virtual void OnEnable()
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _boxCollider2D = gameObject.GetComponent<BoxCollider2D>();

            _boxCollider2D.size = new Vector2(_rectTransform.rect.size.x, _rectTransform.rect.size.y);
        }
    }
}