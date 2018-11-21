using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    public enum TileType
    {
        Exterior,
        NoWall,
        OneWall,
        TwoWalls,
        ThreeWalls
    }

    public int Columns = 25; // The number of columns on the board(how wide it will be).
    public int Rows = 25; // The number of rows on the board(how tall it will be).
    public IntRange NumRooms = new IntRange(4, 5); // The range of the number of rooms there can be.
    public IntRange RoomWidth = new IntRange(3, 6); // The range of widths rooms can have.
    public IntRange RoomHeight = new IntRange(3, 6); // The range of heights rooms can have.
    public GameObject[] NoWallTiles;
    public GameObject[] OneWallTiles;
    public GameObject[] TwoWallTiles;
    public GameObject[] ThreeWallTiles;
    public GameObject[] ExteriorTiles; // An array of exterior tile prefabs.
    public GameObject Player;

    private TileType[][] _tiles; // A jagged array of tile types representing the board, like a grid.
    private Room[] _rooms; // All the rooms.
    private GameObject _boardHolder; // GameObject that acts as a container for all other tiles.

    void Start()
    {
        // Create the board holder.
        _boardHolder = new GameObject("BoardHolder");

        SetupTilesArray();
        CreateRooms();
        SetTilesValuesForRooms();
        InstantiateTiles();
    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        _tiles = new TileType[Columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < _tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            _tiles[i] = new TileType[Rows];
            for (int j = 0; j < Rows; j++)
            {
                _tiles[i][j] = TileType.Exterior;
            }
        }
    }


    void CreateRooms()
    {
        // Create the rooms array with a random size.
        _rooms = new Room[NumRooms.Random];

        _rooms[0] = new Room(RoomWidth, RoomHeight, Columns, Rows);

        for (int i = 1; i < _rooms.Length; i++)
        {
            List<int> availableRooms = new List<int>();
            for (int j = 0; j < i; j++)
            {
                availableRooms.Add(j);
            }

            Room addedRoom;
            do
            {
                if (availableRooms.Count < 1) Debug.Log("Ran out of available rooms during generation!");
                int sourceRoomIndex = Random.Range(0, availableRooms.Count);
                availableRooms.Remove(sourceRoomIndex);
                addedRoom = _rooms[sourceRoomIndex].CreateAdjacentRoom(RoomWidth, RoomHeight);
            } while (addedRoom == null && availableRooms.Count > 0);

            _rooms[i] = addedRoom;
        }
    }


    void SetTilesValuesForRooms()
    {
        // Go through all the rooms...
        for (int i = 0; i < _rooms.Length; i++)
        {
            Room currentRoom = _rooms[i];

            // ... and for each room go through it's width.
            for (int j = 0; j < currentRoom.RoomWidth; j++)
            {
                int xCoord = currentRoom.Coordinates.X + j;

                // For each horizontal tile, go up vertically through the room's height.
                for (int k = 0; k < currentRoom.RoomHeight; k++)
                {
                    int yCoord = currentRoom.Coordinates.Y + k;
                    _tiles[xCoord][yCoord] = currentRoom.GetTileType(xCoord, yCoord);
                }
            }
        }
    }


    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < _tiles.Length; i++)
        {
            for (int j = 0; j < _tiles[i].Length; j++)
            {
                switch (_tiles[i][j])
                {
                    case TileType.Exterior:
                        InstantiateFromArray(ExteriorTiles, i, j);
                        break;
                    case TileType.NoWall:
                        InstantiateFromArray(NoWallTiles, i, j);
                        break;
                    case TileType.OneWall:
                        InstantiateFromArray(OneWallTiles, i, j);
                        break;
                    case TileType.TwoWalls:
                        InstantiateFromArray(TwoWallTiles, i, j);
                        break;
                    case TileType.ThreeWalls:
                        InstantiateFromArray(ThreeWallTiles, i, j);
                        break;
                    default:
                        InstantiateFromArray(ExteriorTiles, i, j);
                        break;
                }
            }
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        // Create a random index for the array.
        int randomIndex = Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = _boardHolder.transform;
    }
}