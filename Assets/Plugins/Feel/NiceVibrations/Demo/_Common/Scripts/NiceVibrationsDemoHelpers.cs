// Copyright (c) Meta Platforms, Inc. and affiliates. 

using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public static class NiceVibrationsDemoHelpers
    {
        public static float Round(float value, int digits)
        {
            var mult = Mathf.Pow(10.0f, digits);
            return Mathf.Round(value * mult) / mult;
        }

        public static float Remap(float x, float A, float B, float C, float D)
        {
            var remappedValue = C + (x - A) / (B - A) * (D - C);
            return remappedValue;
        }
    }
}