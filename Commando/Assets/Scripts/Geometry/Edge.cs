using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Geometry {
    public class Edge
    {

        public Vector3 V1;
        public Vector3 V2;

        public Edge(Vector3 v1, Vector3 v2)
        {
            V1 = v1;
            V2 = v2;
        }
    }
}
