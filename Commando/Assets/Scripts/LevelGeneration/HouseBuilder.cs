using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Geometry;
using Assets.Scripts.Helpers;
using JetBrains.Annotations;
using UnityEngine;
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
    }
}