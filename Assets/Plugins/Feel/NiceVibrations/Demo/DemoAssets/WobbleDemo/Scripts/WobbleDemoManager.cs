// Copyright (c) Meta Platforms, Inc. and affiliates. 

using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations
{
    public class WobbleDemoManager : DemoManager
    {
        public Camera ButtonCamera;
        public RectTransform ContentZone;
        public WobbleButton WobbleButtonPrefab;
        public Vector2 PrefabSize = new(200f, 200f);
        public float Margin = 20f;
        public float Padding = 20f;
        protected Canvas _canvas;
        protected Vector3 _position = Vector3.zero;

        protected List<WobbleButton> Buttons;

        protected virtual void Start()
        {
            _canvas = GetComponentInParent<Canvas>();

            var horizontalF = (ContentZone.rect.width - 2 * Padding) / (PrefabSize.x + Margin);
            var verticalF = (ContentZone.rect.height - 2 * Padding) / (PrefabSize.y + Margin);
            var horizontal = Mathf.FloorToInt(horizontalF);
            var vertical = Mathf.FloorToInt(verticalF);

            var centerH =
                (ContentZone.rect.width - Padding * 2 - horizontal * PrefabSize.x - (horizontal - 1) * Margin) / 2f;
            var centerV = (ContentZone.rect.height - Padding * 2 - vertical * PrefabSize.x - (vertical - 1) * Margin) /
                          2f;

            Buttons = new List<WobbleButton>();

            for (var i = 0; i < horizontal; i++)
            for (var j = 0; j < vertical; j++)
            {
                _position.x = centerH + Padding + PrefabSize.x / 2f + i * (PrefabSize.x + Margin);
                _position.y = centerV + Padding + PrefabSize.y / 2f + j * (PrefabSize.y + Margin);
                _position.z = 0f;

                var button = Instantiate(WobbleButtonPrefab);
                button.transform.SetParent(ContentZone.transform);
                Buttons.Add(button);

                var rectTransform = button.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                button.name = "WobbleButton" + i + j;
                button.transform.localScale = Vector3.one;

                rectTransform.anchoredPosition3D = _position;
                button.TargetCamera = ButtonCamera;
                button.Initialization();
            }

            var counter = 0;
            foreach (var wbutton in Buttons)
            {
                var newPitch = NiceVibrationsDemoHelpers.Remap(counter, 0f, Buttons.Count, 0.3f, 1f);
                wbutton.SetPitch(newPitch);
                counter++;
            }
        }
    }
}