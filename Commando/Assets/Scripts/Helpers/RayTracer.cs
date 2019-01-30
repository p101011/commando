using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class RayTracer : MonoBehaviour
    {

        private static GameObject _rayHolder;
        private static int _raysCast;

        public void Start()
        {
            _rayHolder = new GameObject("RayHolder");
            LoSRayCast(Vector3.zero, Vector3.right);
        }

        public static RaycastHit2D[] LoSRayCast(Vector3 origin, Vector3 direction)
        {
            // LoS targets are on layer 8
            const int layerBitMask = 1 << 8;
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, float.PositiveInfinity, layerBitMask);
            if (GameVariables.Debug) DrawRay(origin, direction.normalized * GameVariables.XRes, Color.magenta, 0.5f);
            _raysCast++;
            return hits;
        }

        private static void DrawRay(Vector3 start, Vector3 end, Color color, float width) {
            GameObject myLine = new GameObject($"Ray #{_raysCast}");
            myLine.transform.parent = _rayHolder.transform;
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.sortingLayerName = "Foreground";
            lr.sortingOrder = 1;

            Destroy(myLine, 3);
        }
    }
}
