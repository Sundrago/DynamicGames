using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetKits.ParticleImage
{
    [AddComponentMenu("UI/Particle Image/Trail Renderer")]
    public class ParticleTrailRenderer : MaskableGraphic
    {
        private readonly List<UIVertex> _vertexList = new();

        public ParticleImage particle { get; set; }

        public VertexHelper vertexHelper { get; set; } = new();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            vertexHelper.GetUIVertexStream(_vertexList);

            vh.AddUIVertexTriangleStream(_vertexList);

            vertexHelper.Clear();
        }
    }
}