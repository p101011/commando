using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Data {
    public struct VisualCone
    {

        public Vector3[] Vertices;

        public VisualCone(Vector3 origin, Vector3 rightPoint, Vector3 leftPoint)
        {
            Vertices = new[] { origin, rightPoint, leftPoint };
        }

        public void AddVertex(Vector3 newVertex)
        {
            Array.Resize(ref Vertices, Vertices.Length + 1);
            Vertices[Vertices.Length - 1] = newVertex;
        }

    }
}
