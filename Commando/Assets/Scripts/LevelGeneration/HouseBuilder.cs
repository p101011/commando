using System;
using System.Collections.Generic;
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

        [UsedImplicitly]
        private void Start()
        {
            //todo: use room prefabs rather than full random
            //todo: dynamically build walls after rooms picked
            //todo: use GameVariables.X/YRes to figure out the size of our board

            // Create the board holder.
            _boardHolder = new GameObject("BoardHolder");
            CreateBuildings();
            InstantiateBackgroundTiles();
//            InstantiateBuildingFloor();
//            InstantiateWalls();
        }

        private void InstantiateWalls()
        {
            throw new System.NotImplementedException();
            foreach (Building b in Buildings) {
            }
        }

        private void InstantiateBuildingFloor()
        {
            throw new NotImplementedException();
            foreach (Building b in Buildings)
            {
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

        private void InstantiateBackgroundTiles()
        {
            for (int i = 0; i <= GameVariables.XRes / GameConstants.TileSize; i++)
            {
                for (int j = 0; j <= GameVariables.YRes / GameConstants.TileSize; j++) {
                    int randomIndex = Random.Range(0, ExteriorTiles.Length);
                    Vector3 position = new Vector3(i, j, 0f);
                    GameObject tileInstance = Instantiate(ExteriorTiles[randomIndex], position, Quaternion.identity);
                    tileInstance.transform.parent = _boardHolder.transform;
                }
            }
        }

        public Vector3 GetCoordinates(string identifier)
        {
            return new Vector3();
        }
    }
}