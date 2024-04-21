// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class BallDemoManager : DemoManager
    {
        [Header("Ball")] public Vector2 Gravity = new(0, -30f);

        protected virtual void Start()
        {
            Physics2D.gravity = Gravity;
        }
    }
}