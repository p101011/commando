using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Geometry
{
    public class Polygon
    {
        public List<Vector3> Vertices;
        public List<Edge> Edges;
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;
        public Vector3 Center;

        public Polygon(List<Edge> edges)
        {
            Edges = edges;
            Vertices = new List<Vector3>();
            float avgX = 0;
            float avgY = 0;
            foreach (Edge e in edges)
            {
                if (!Vertices.Contains(e.V1))
                {
                    Vertices.Add(e.V1);
                    avgX += e.V1.x;
                    avgY += e.V1.y;
                    if (MinX > e.V1.x)
                    {
                        MinX = e.V1.x;
                    }
                    else if (MaxX < e.V1.x)
                    {
                        MaxX = e.V1.x;
                    }
                    if (MinY > e.V1.y) {
                        MinY = e.V1.y;
                    }
                    else if (MaxY < e.V1.y) {
                        MaxY = e.V1.y;
                    }
                }

                if (Vertices.Contains(e.V2)) continue;
                Vertices.Add(e.V2);
                avgX += e.V2.x;
                avgY += e.V2.y;
                if (MinX > e.V2.x) {
                    MinX = e.V2.x;
                }
                else if (MaxX < e.V2.x) {
                    MaxX = e.V2.x;
                }
                if (MinY > e.V2.y) {
                    MinY = e.V2.y;
                }
                else if (MaxY < e.V2.y) {
                    MaxY = e.V2.y;
                }
            }

            avgY /= Vertices.Count;
            avgX /= Vertices.Count;
            Center = new Vector3(avgX, avgY);
        }

        public Polygon(Polygon other)
        {
            Vertices = new List<Vector3>(other.Vertices);
            Edges = new List<Edge>(other.Edges);
            Center = new Vector3(other.Center.x, other.Center.y);
            MinX = other.MinX;
            MinY = other.MinY;
            MaxX = other.MaxX;
            MaxY = other.MaxY;
        }

        public Polygon(Polygon a, Polygon b)
        {
            foreach (Edge e1 in a.Edges)
            {
                foreach (Edge e2 in b.Edges)
                {
                    if (e1.Equals(e2))
                    {
                        b.Edges.Remove(e2);
                    }
                }
            }

            foreach (Vector3 v1 in a.Vertices)
            {
                foreach (Vector3 v2 in b.Vertices)
                {
                    if (v1.Equals(v2))
                    {
                        b.Vertices.Remove(v2);
                    }
                }
            }

            Vertices = new List<Vector3>(a.Vertices);
            Edges = new List<Edge>(a.Edges);
            Vertices.AddRange(b.Vertices);
            Edges.AddRange(b.Edges);

            float avgX = 0;
            float avgY = 0;
            foreach (Vector3 v in Vertices)
            {
                avgX += v.x;
                avgY += v.y;
            }

            avgY /= Vertices.Count;
            avgX /= Vertices.Count;
            Center = new Vector3(avgX, avgY);
            MinX = Mathf.Min(a.MinX, b.MinX);
            MinY = Mathf.Min(a.MinY, b.MinY);
            MaxX = Mathf.Min(a.MaxX, b.MaxX);
            MaxY = Mathf.Min(a.MaxY, b.MaxY);
        }

        public bool Intersects(Polygon other)
        {
            foreach (Polygon polygon in new[] { this, other }) {
                for (int i1 = 0; i1 < polygon.Vertices.Count; i1++) {
                    int i2 = (i1 + 1) % polygon.Vertices.Count;
                    Vector3 p1 = polygon.Vertices[i1];
                    Vector3 p2 = polygon.Vertices[i2];

                    Vector3 normal = new Vector3(p2.y - p1.y, p1.x - p2.x);

                    double? minA = null, maxA = null;
                    foreach (Vector3 p in Vertices) {
                        float projected = normal.x * p.x + normal.y * p.y;
                        if (minA == null || projected < minA)
                            minA = projected;
                        if (maxA == null || projected > maxA)
                            maxA = projected;
                    }

                    double? minB = null, maxB = null;
                    foreach (Vector3 p in other.Vertices) {
                        float projected = normal.x * p.x + normal.y * p.y;
                        if (minB == null || projected < minB)
                            minB = projected;
                        if (maxB == null || projected > maxB)
                            maxB = projected;
                    }

                    if (maxA < minB || maxB < minA)
                        return false;
                }
            }
            return true;
        }

        public void Rotate(int angle)
        {
            float radians = Mathf.Deg2Rad * angle;
            foreach (Vector3 t in Vertices)
            {
                Vector3 vertex = t;
                // rotation pivot will be from center
                vertex.x = Mathf.Cos(radians) * (vertex.x - Center.x) - Mathf.Sin(radians) * (vertex.y - Center.y) +
                           Center.x;
                vertex.y = Mathf.Sin(radians) * (vertex.x - Center.x) - Mathf.Cos(radians) * (vertex.y - Center.y) +
                           Center.y;
            }
            foreach (Edge e in Edges) {
                Vector3 vertex = e.V1;
                vertex.x = Mathf.Cos(radians) * (vertex.x - Center.x) - Mathf.Sin(radians) * (vertex.y - Center.y) +
                           Center.x;
                vertex.y = Mathf.Sin(radians) * (vertex.x - Center.x) - Mathf.Cos(radians) * (vertex.y - Center.y) +
                           Center.y;
                vertex = e.V2;
                vertex.x = Mathf.Cos(radians) * (vertex.x - Center.x) - Mathf.Sin(radians) * (vertex.y - Center.y) +
                           Center.x;
                vertex.y = Mathf.Sin(radians) * (vertex.x - Center.x) - Mathf.Cos(radians) * (vertex.y - Center.y) +
                           Center.y;
            }
        }

        public void Translate(Vector3 translation)
        {
            foreach (Vector3 vertex in Vertices)
            {
                Vector3 vector3 = vertex;
                vector3 += translation;
            }
            foreach (Edge e in Edges)
            {
                e.V1 += translation;
                e.V2 += translation;
            }
        }
    }
}