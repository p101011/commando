using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class RayTracer : MonoBehaviour
    {

        private static GameObject _rayHolder;
        private static int _raysCast;
        private static int _layerBitMask = 0;

        private static readonly float RayLength = Vector3.Distance(new Vector3(GameVariables.XRes, 0), new Vector3(0, GameVariables.YRes));

        public void Start()
        {
            _rayHolder = new GameObject("RayHolder");
            string[] losLayers = { "LoSTarget", "OpaqueLoSTarget" };
            foreach (string layerName in losLayers)
            {
                int layerIndex = LayerMask.NameToLayer(layerName);
                if (layerIndex == -1 && GameVariables.Debug) Debug.LogError($"Failed to find layer {layerName}");
                else
                {
                    _layerBitMask += 1 << layerIndex;
                }
            }
        }

        public static RaycastHit2D[] LoSRayCast(Vector3 origin, Vector3 direction)
        {
            RaycastHit2D[] hits = Physics2D.LinecastAll(origin, origin + direction * RayLength, _layerBitMask);
            if (GameVariables.Debug)
            {
                Color rayColor = hits.Length > 1 ? Color.red : Color.magenta;
                DrawRay(origin, origin + direction * RayLength, rayColor, 1f);
            }
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
