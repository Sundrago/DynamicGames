using System;
using System.Text;
using UnityEngine;

namespace Febucci.UI.Core
{
    public struct MeshData : IEquatable<MeshData>
    {
        public Vector3[] positions;
        public Color32[] colors;


        public bool Equals(MeshData other)
        {
            //checks every position with other's position
            for (var i = 0; i < positions.Length; i++)
                if (positions[i] != other.positions[i])
                    return false;

            //checks every color with other's color
            for (var i = 0; i < colors.Length; i++)
                if (!colors[i].Equals(other.colors[i]))
                    return false;

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            //prints positions and colors
            for (var i = 0; i < positions.Length; i++)
            {
                sb.Append(positions[i].ToString());
                sb.Append(" ");
                sb.Append(colors[i].ToString());
                sb.Append(" - ");
            }

            return sb.ToString();
        }
    }
}