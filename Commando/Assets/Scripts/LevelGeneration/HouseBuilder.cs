using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Assets.Scripts.Geometry;
using Assets.Scripts.Helpers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = System.Diagnostics.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.LevelGeneration
{
    public class HouseBuilder : MonoBehaviour
    {
        public static IntRange NumBuildings = new IntRange(1, 1);
        public List<Building> Buildings = new List<Building>();

        // prefabs for tiles
        public GameObject[] InteriorTiles;
        public GameObject[] ExteriorTiles; // An array of exterior tile prefabs.
        private GameObject _boardHolder; // GameObject that acts as a container for all other tiles.
        private GameObject _wallHolder;
        private GameObject _floorHolder;

        [UsedImplicitly]
        private void Start()
        {

            // Create the board holder.
            _boardHolder = new GameObject("BoardHolder");
            _wallHolder = new GameObject("WallHolder");
            _floorHolder = new GameObject("FloorHolder");
            TestRoomRotation();
            CreateBuildings();
            InstantiateBackgroundTiles();
            InstantiateWalls();
            InstantiateBuildingFloor();
        }

        private void InstantiateWalls()
        {
            foreach (Building b in Buildings) 
            {
                foreach (Room r in b.Rooms)
                {
                    foreach (Edge e in r.BoundingPolygon.Edges)
                    {
                        DrawLine(e.V1, e.V2, GameConstants.WallColor, GameConstants.WallThickness);
                    }
                }
            }
        }

        private void InstantiateBuildingFloor()
        {
            foreach (Building b in Buildings)
            {
                Polygon floorBounds = b.BoundingPolygon;
                for (float x = floorBounds.MinX; x < floorBounds.MaxX; x += GameConstants.TileSize)
                {
                    for (float y = floorBounds.MinY; y < floorBounds.MaxY; y += GameConstants.TileSize) {
                        int randomIndex = Random.Range(0, InteriorTiles.Length);
                        Vector3 position = new Vector3(x, y);
                        GameObject tileInstance = Instantiate(InteriorTiles[randomIndex], position, Quaternion.identity);
                        tileInstance.transform.parent = _floorHolder.transform;
                    }
                }
            }
        }

        private void CreateBuildings()
        {
            int numBuildings = NumBuildings.Random;
            for (int i = 0; i < numBuildings; i++)
            {
                Vector3 entranceCoordinates = new Vector3((float) GameVariables.XRes / 2, (float) GameVariables.YRes / 2);
                Buildings.Add(new Building(i, entranceCoordinates));
            }
        }

        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        private void InstantiateBackgroundTiles()
        {
            for (int i = 0; i <= Mathf.CeilToInt(GameVariables.XRes / GameConstants.TileSize); i++)
            {
                for (int j = 0; j <= Mathf.CeilToInt(GameVariables.YRes / GameConstants.TileSize); j++) {
                    int randomIndex = Random.Range(0, ExteriorTiles.Length);
                    Vector3 position = new Vector3(i * GameConstants.TileSize, j * GameConstants.TileSize, 0f);
                    GameObject tileInstance = Instantiate(ExteriorTiles[randomIndex], position, Quaternion.identity);
                    tileInstance.transform.parent = _boardHolder.transform;
                }
            }
        }

        public Vector3 GetCoordinates(string identifier)
        {
            return new Vector3();
        }

        private void DrawLine(Vector3 start, Vector3 end, Color color, float width)
        {
            GameObject myLine = new GameObject("Wall");
            myLine.transform.parent = _wallHolder.transform;
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
            lr.sortingOrder = 5;
        }

        private static void TestRoomRotation()
        {
            Vector3 ll = new Vector3(0, 0);
            Vector3 lr = new Vector3(1, 0);
            Vector3 ul = new Vector3(0, 1);
            Vector3 ur = new Vector3(1, 1);
            List<Edge> testEdges =
                new List<Edge> {new Edge(ll, lr), new Edge(lr, ur), new Edge(ur, ul), new Edge(ul, ll)};
            List<PointOfInterest> testPoints = new List<PointOfInterest> { new PointOfInterest(PointOfInterest.PoIType.Door, PointOfInterest.Facing.South, true, new Vector3(.5f, 0)) };
            RoomTemplate testTemplate = new RoomTemplate(testEdges.ToArray(), testPoints);
            Room testRoom = new Room(testTemplate);
            List<Edge> expectedEdges =
                new List<Edge> { new Edge(ur, ul), new Edge(ul, ll), new Edge(ll, lr), new Edge(lr, ur) };
            List<PointOfInterest> expectedPoints = new List<PointOfInterest> { new PointOfInterest(PointOfInterest.PoIType.Door, PointOfInterest.Facing.North, true, new Vector3(.5f, 1)) };
            testRoom.Rotate(180);
            for (int i = 0; i < 4; i++)
            {
                Assert.IsTrue(testRoom.BoundingPolygon.Edges[i] == expectedEdges[i], $"Edge rotated improperly, expected {expectedEdges[i]} got {testRoom.BoundingPolygon.Edges[i]}");
            }
            Assert.IsTrue(expectedPoints[0].Coordinates == testRoom.KeyPoints[0].Coordinates, "Keypoint Rotated improperly, expected " + expectedPoints[0].Coordinates + " got " + testRoom.KeyPoints[0].Coordinates);
        }
    }
}