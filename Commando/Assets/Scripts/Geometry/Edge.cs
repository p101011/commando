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

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Edge)) return false;
            Edge eObj = (Edge) obj;
            return  V1 == eObj.V1 && V2 == eObj.V2;
        }

        public override int GetHashCode()
        {
            return V1.GetHashCode() + V2.GetHashCode() << 2;
        }

        protected static bool Equals(Edge left, Edge right)
        {
            return left.V1 == right.V1 && left.V2 == right.V2;
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"{V1} -> {V2}";
        }
    }
}
