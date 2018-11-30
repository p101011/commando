using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Geometry;
using Assets.Scripts.Helpers;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration {
    public class RoomTemplate
    {

        public Edge[] Edges;
        public List<PointOfInterest> KeyPoints;
        private const string TemplatePath = "./Assets/Scripts/LevelGeneration/RoomTemplates.json";
        private static readonly JObject ParsedTemplate = JObject.Parse(File.ReadAllText(TemplatePath));
        private static List<JToken> _shuffledTemplates;

        public RoomTemplate(Edge[] edges, List<PointOfInterest> keyPoints)
        {
            Edges = edges;
            KeyPoints = keyPoints;
        }


        public static RoomTemplate GetRoomTemplate(Room.RoomType type, int numDoorsConnecting, bool newTry) {

            if (newTry || _shuffledTemplates == null) {
                JToken roomToken = null;
                IJEnumerable<JToken> templates = null;
                foreach (KeyValuePair<string, JToken> kvp in ParsedTemplate)
                {
                    if (kvp.Key != type.ToString()) continue;
                    roomToken = kvp.Value;
                    break;
                }

                if (roomToken == null)
                {
                    Debug.Log("GetRoomTemplate did not find a room of type " + type);
                    return null;
                }

                foreach (JToken doorGroup in roomToken.Children())
                {
                    if (doorGroup["NumDoors"].Value<int>() != numDoorsConnecting) continue;
                    templates = doorGroup["Templates"];
                    break;
                }

                if (templates == null) {
                    Debug.Log("GetRoomTemplate did not find a room of type " + type + " which connects to " + numDoorsConnecting + " doors");
                    return null;
                }
                _shuffledTemplates = templates.OrderBy(x => Guid.NewGuid()).ToList();
            }

            if (_shuffledTemplates.Count == 0)
            {
                return null;
            }

            JToken selectedToken = _shuffledTemplates.First();
            _shuffledTemplates.RemoveAt(0);
            List<Edge> list = new List<Edge>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (JToken edge in selectedToken["Edges"])
            {
                List<JToken> edgeS = edge.Values().ToList();
                List<int> edgeP = new List<int>();
                foreach (JToken s in edgeS)
                {
                    // json stores positions as relative coordinates, need to scale to be correct for scene
                    int rawValue = s.ToObject<int>();
                    edgeP.Add(Mathf.RoundToInt(rawValue * GameConstants.RoomScalar));
                }
                Vector3 v1 = new Vector3(edgeP[0], edgeP[1]);
                Vector3 v2 = new Vector3(edgeP[2], edgeP[3]);
                list.Add(new Edge(v1, v2));
            }

            Edge[] edgeArray = list.ToArray();
            List<PointOfInterest> pois = new List<PointOfInterest>();
            foreach (JToken keypoint in selectedToken["KeyPoints"])
            {
                List<int> point = keypoint["Point"].Values<int>().ToList();
                point.ForEach(x => Mathf.RoundToInt(x * GameConstants.RoomScalar));
                Vector3 position = new Vector3(point[0], point[1]);
                PointOfInterest.PoIType pointType = PointOfInterest.PoIType.Door;
                bool pointAvailable = keypoint["Available"].Value<bool>();
                PointOfInterest.Facing pointFacing;
                switch (keypoint["Facing"].Value<string>()) {
                    case "North":
                        pointFacing = PointOfInterest.Facing.North;
                        break;
                    case "South":
                        pointFacing = PointOfInterest.Facing.South;
                        break;
                    case "East":
                        pointFacing = PointOfInterest.Facing.East;
                        break;
                    case "West":
                        pointFacing = PointOfInterest.Facing.West;
                        break;
                    default:
                        Debug.Log("GetRoomTemplate received a bad argument for KeyPoint.Facing");
                        return null;
                }
                pois.Add(new PointOfInterest(pointType, pointFacing, pointAvailable, position));
            }
            return new RoomTemplate(edgeArray, pois);
        }
    }
}
