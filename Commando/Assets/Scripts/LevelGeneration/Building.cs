using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Assets.Scripts.Helpers;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.LevelGeneration {


    public class Building : Location {

        public Room[] Rooms;
        public int NumRooms;
        public int Id;
        public Coordinates EntranceCoordinates;
        private readonly Tile[][] _tiles;
        public Dictionary<string, Tile> KeyTiles = new Dictionary<string, Tile>();

        public Building(int numRooms, Tile[][] tiles, int id, Coordinates doorCoordinates)
        {
            NumRooms = numRooms;
            Rooms = new Room[numRooms];
            _tiles = tiles;
            Id = id;
            EntranceCoordinates = doorCoordinates;

            CreateRooms();
        }

        private void CreateRooms()
        {

            KeyTiles.Add("MainEntrance", GetTileAtPosition(EntranceCoordinates));

            Rooms[0] = new Room(HouseBuilder.RoomWidth, HouseBuilder.RoomHeight, EntranceCoordinates);

            for (int i = 1; i < NumRooms; i++) {
                List<int> availableRooms = new List<int>();
                for (int j = 0; j < i; j++) {
                    availableRooms.Add(j);
                }

                Room addedRoom;
                do {
                    if (availableRooms.Count < 1) Debug.Log("Ran out of available rooms during generation!");
                    int sourceRoomIndex = UnityEngine.Random.Range(0, availableRooms.Count);
                    availableRooms.Remove(sourceRoomIndex);
                    addedRoom = Rooms[sourceRoomIndex].CreateAdjacentRoom(HouseBuilder.RoomWidth, HouseBuilder.RoomHeight);
                } while (addedRoom == null && availableRooms.Count > 0);

                if (addedRoom == null) return;

                Rooms[i] = addedRoom;

                for (int row = (int)addedRoom.Coordinates.X; row < addedRoom.Coordinates.X + addedRoom.RoomWidth; row++) {
                    for (int col = (int)addedRoom.Coordinates.Y; col < addedRoom.Coordinates.Y + addedRoom.RoomHeight; col++) {
                        _tiles[row][col].Type = Tile.TileType.NoWall;
                    }
                }
            }
        }

        public Tile GetTileById(string id)
        {
            Tile output;

            if (KeyTiles.TryGetValue(id, out output))
            {
                return output;
            }

            Debug.Log("Failed to find tile " + id);
            return null;
        }

        public Tile GetTileAtPosition(Coordinates coordinates) {
            int roundedX = (int)coordinates.X;
            int roundedY = (int)coordinates.Y;
            return _tiles[roundedX][roundedY];
        }
    }
}
