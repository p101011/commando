using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration
{
    public class HouseBuilder : MonoBehaviour
    {
        public static IntRange NumBuildings = new IntRange(1, 1);
        public List<Building> Buildings = new List<Building>();

        // prefabs for tiles
        public GameObject[] InteriorTiles;
        public GameObject[] ExteriorTiles; // An array of exterior tile prefabs.

        private BackgroundTile[][] _backgroundTiles; // A jagged array of tile types representing the background
        private GameObject _boardHolder; // GameObject that acts as a container for all other tiles.

        private void Start()
        {
            //todo: use room prefabs rather than full random
            //todo: dynamically build walls after rooms picked
            //todo: use GameVariables.X/YRes to figure out the size of our board

            // Create the board holder.
            _boardHolder = new GameObject("BoardHolder");
            SetupTilesArray();
            CreateBuildings();
            InstantiateTiles();
        }


        private void SetupTilesArray()
        {
            int numTilesWide = GameVariables.XRes / GameConstants.TileSize;
            int numTilesTall = GameVariables.YRes / GameConstants.TileSize;
            // Set the tiles jagged array to the correct width.
            _backgroundTiles = new BackgroundTile[numTilesWide][];

            // Go through all the tile arrays...
            for (int i = 0; i < _backgroundTiles.Length; i++)
            {
                // ... and set each tile array is the correct height.
                _backgroundTiles[i] = new BackgroundTile[numTilesTall];
                for (int j = 0; j < numTilesTall; j++)
                {
                    _backgroundTiles[i][j] = new BackgroundTile();
                }
            }
        }

        private void CreateBuildings()
        {
            int numBuildings = NumBuildings.Random;
            for (int i = 0; i < numBuildings; i++)
            {
                Vector3 entranceCoordinates = new Vector3((float) GameVariables.XRes / 2, (float) GameVariables.YRes / 2);
                Buildings.Add(new Building(_backgroundTiles, i, entranceCoordinates));
            }
        }


        private void InstantiateTiles()
        {
            // Go through all the tiles in the jagged array...
            for (int i = 0; i < _backgroundTiles.Length; i++)
            {
                for (int j = 0; j < _backgroundTiles[i].Length; j++)
                {
                    switch (_backgroundTiles[i][j].Type)
                    {
                        case BackgroundTile.TileType.Exterior:
                            InstantiateFromArray(ExteriorTiles, i, j);
                            break;
                        case BackgroundTile.TileType.Interior:
                            InstantiateFromArray(InteriorTiles, i, j);
                            break;
                        default:
                            InstantiateFromArray(ExteriorTiles, i, j);
                            break;
                    }
                }
            }
        }

        private void InstantiateFromArray(IList<GameObject> prefabs, float xCoord, float yCoord)
        {
            // Create a random index for the array.
            int randomIndex = Random.Range(0, prefabs.Count);

            // The position to be instantiated at is based on the coordinates.
            Vector3 position = new Vector3(xCoord, yCoord, 0f);

            // Create an instance of the prefab from the random index of the array.
            GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity);

            // Set the tile's parent to the board holder.
            tileInstance.transform.parent = _boardHolder.transform;
        }

        public BackgroundTile GetTile(string id)
        {
            return Buildings[0].GetTileById(id);
        }
    }
}