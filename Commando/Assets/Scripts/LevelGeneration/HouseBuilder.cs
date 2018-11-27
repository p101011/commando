using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration
{
    public class HouseBuilder : MonoBehaviour
    {

        public static int Columns = 25; // The number of columns on the board(how wide it will be).
        public static int Rows = 25; // The number of rows on the board(how tall it will be).
        public static IntRange NumRooms = new IntRange(4, 5); // The range of the number of rooms there can be.
        public static IntRange NumBuildings = new IntRange(1, 1);
        public static IntRange RoomWidth = new IntRange(3, 6); // The range of widths rooms can have.
        public static IntRange RoomHeight = new IntRange(3, 6); // The range of heights rooms can have.
        public List<Building> Buildings = new List<Building>();

        // prefabs for tiles
        public GameObject[] NoWallTiles;
        public GameObject[] ExteriorTiles; // An array of exterior tile prefabs.

        private Tile[][] _tiles; // A jagged array of tile types representing the board, like a grid.
        private GameObject _boardHolder; // GameObject that acts as a container for all other tiles.

        private void Start()
        {
            // Create the board holder.
            _boardHolder = new GameObject("BoardHolder");
            SetupTilesArray();
            CreateBuildings();
            InstantiateTiles();
        }


        private void SetupTilesArray()
        {
            // Set the tiles jagged array to the correct width.
            _tiles = new Tile[Columns][];

            // Go through all the tile arrays...
            for (int i = 0; i < _tiles.Length; i++)
            {
                // ... and set each tile array is the correct height.
                _tiles[i] = new Tile[Rows];
                for (int j = 0; j < Rows; j++)
                {
                    _tiles[i][j] = new Tile(new Coordinates(i, j));
                }
            }
        }

        private void CreateBuildings()
        {
            int numBuildings = NumBuildings.Random;
            for (int i = 0; i < numBuildings; i++)
            {
                Coordinates entranCoordinates = new Coordinates(Rows / 2, Columns / 2);
                Buildings.Add(new Building(NumRooms.Random, _tiles, i, entranCoordinates));
            }
        }


        private void InstantiateTiles()
        {
            // Go through all the tiles in the jagged array...
            for (int i = 0; i < _tiles.Length; i++)
            {
                for (int j = 0; j < _tiles[i].Length; j++)
                {
                    switch (_tiles[i][j].Type)
                    {
                        case Tile.TileType.Exterior:
                            InstantiateFromArray(ExteriorTiles, i, j);
                            break;
                        case Tile.TileType.NoWall:
                            InstantiateFromArray(NoWallTiles, i, j);
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

        public Tile GetTileAtPosition(Coordinates coordinates)
        {
            int roundedX = (int) coordinates.X;
            int roundedY = (int) coordinates.Y;
            return _tiles[roundedX][roundedY];
        }

        public Tile GetTile(string id)
        {
            return Buildings[0].GetTileById(id);
        }
    }
}