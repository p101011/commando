using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.LevelGeneration {


    public class Building : Location 
    {
        public List<Room> Rooms;
        public int Id;
        public Vector3 EntranceCoordinates;
        public Dictionary<string, BackgroundTile> KeyTiles = new Dictionary<string, BackgroundTile>();

        public Building(IList<BackgroundTile[]> tiles, int id, Vector3 doorCoordinates)
        {
            Rooms = new List<Room>();
            Id = id;
            EntranceCoordinates = doorCoordinates;

            CreateRooms(tiles);
        }

        private void CreateRooms(IList<BackgroundTile[]> tiles)
        {

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
                        tiles[row][col].Type = BackgroundTile.TileType.NoWall;
                    }
                }
            }
        }

        public BackgroundTile GetTileById(string id)
        {
            BackgroundTile output;

            if (KeyTiles.TryGetValue(id, out output))
            {
                return output;
            }

            Debug.Log("Failed to find tile " + id);
            return null;
        }

        public BackgroundTile GetTileAtPosition(Coordinates coordinates) {
            int roundedX = (int)coordinates.X;
            int roundedY = (int)coordinates.Y;
            return _tiles[roundedX][roundedY];
        }
    }
}
