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
        public GameObject ExteriorTile; // An array of exterior tile prefabs.
        public GameObject WallPrefab;
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
                        Vector3 center = (e.V1 + e.V2) / 2;
                        float angle = Math.Abs(Vector3.Angle(e.V2 - e.V1, Vector3.right) % 180);
                        GameObject wallInstance = Instantiate(WallPrefab, center, Quaternion.identity);
                        // wall prefab has sprite that is 72 px tall; need to adjust length to match
                        float wallLength = Vector3.Distance(e.V1, e.V2) / GameConstants.TileSize;
                        float wallWidth = (float)GameConstants.WallThickness / GameConstants.TileSize;
                        wallInstance.transform.parent = _wallHolder.transform;
                        // we can't just rotate colliders by 90 degrees for some stupid reason
                        if (Math.Abs(angle - 180) < float.Epsilon || Math.Abs(angle - 90) < float.Epsilon)
                        {
                            wallInstance.GetComponent<BoxCollider2D>().size = new Vector2(GameConstants.WallThickness / wallWidth,
                                Vector3.Distance(e.V1, e.V2) / wallLength);
                            wallInstance.transform.localScale =
                                new Vector3(wallWidth, wallLength);
                        }
                        else
                        {
                            wallInstance.GetComponent<BoxCollider2D>().size = new Vector2(Vector3.Distance(e.V1, e.V2) / wallLength,
                                GameConstants.WallThickness / wallWidth);
                            wallInstance.transform.localScale =
                                new Vector3(wallLength, wallWidth);
                        }
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
            Vector3 position = new Vector3((float) (GameVariables.XRes / 2.0), (float) (GameVariables.YRes / 2.0));
            GameObject tileInstance = Instantiate(ExteriorTile, position, Quaternion.identity);
            tileInstance.transform.parent = _boardHolder.transform;
            tileInstance.transform.localScale = new Vector3(GameVariables.XRes / GameConstants.TileSize, GameVariables.YRes / GameConstants.TileSize);
        }

        public Vector3 GetCoordinates(string identifier)
        {
            return new Vector3();
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