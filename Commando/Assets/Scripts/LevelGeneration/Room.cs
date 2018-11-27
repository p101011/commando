using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration
{
    public class Room : Location
    {
        public enum RoomType
        {
            Foyer, LivingRoom, Bathroom, Kitchen, Office
        }

        public List<Vector3> Vertices;
        public List<Room> AdjacentRooms;

        private readonly int[] _nextOpenSpaces = {0, 0, 0, 0};  // {N, E, S, W} offset from left tile
        
        public Room(RoomType)
        {
            Coordinates = anchor;
            AdjacentRooms = new List<Room>();
        }

        public Room CreateAdjacentRoom(IntRange widthRange, IntRange heightRange) 
        {
            int startingSide = Random.Range(0, 3);
            bool foundCoord = false;
            int i = startingSide;
            do
            {
                int maxOffset = i % 2 == 0 ? RoomWidth : RoomHeight; // if i is 0, i is a horizontal side
                if (_nextOpenSpaces[i] < maxOffset) { foundCoord = true; }
                else { i = (i + 1) % 4; }
            }
            while (!foundCoord && i != startingSide);
            if (!foundCoord) { return null; }
            bool foundWallIsHorizontal = i % 2 == 0;
            int xOffset = 0;
            int yOffset = 0;
            if (foundWallIsHorizontal) { xOffset = _nextOpenSpaces[i]; }
            else { yOffset = _nextOpenSpaces[i]; }
            double xPos = Coordinates.X + xOffset;
            double yPos = Coordinates.Y + yOffset;
            Coordinates coords = new Coordinates(xPos, yPos);
            Room builtRoom = new Room(widthRange, heightRange, coords);
            int addedBlocker = i % 2 == 0 ? builtRoom.RoomWidth : builtRoom.RoomHeight;
            _nextOpenSpaces[i] += addedBlocker;
            AdjacentRooms.Add(builtRoom);
            return builtRoom;
        }

        public BackgroundTile.TileType GetTileType(int x, int y)
        {
            List<bool> walls = new List<bool> {false, false, false, false};
            if (x > Coordinates.X + RoomWidth || y > Coordinates.Y + RoomHeight || x < Coordinates.X || y < Coordinates.Y) {
                throw new System.ArgumentOutOfRangeException("The given coordinates aren't inside the room!");
            }
            if (x == Coordinates.X) walls[3] = true;
            if (x == Coordinates.X + RoomWidth) walls[1] = true;
            if (y == Coordinates.Y) walls[2] = true;
            if (y == Coordinates.Y + RoomWidth) walls[0] = true;
            switch (walls.Count(b => b))
            {
                case 0:
                    return BackgroundTile.TileType.NoWall;
                case 1:
                    return BackgroundTile.TileType.Exterior;
                case 2:
                    return BackgroundTile.TileType.Exterior;
                case 3:
                    return BackgroundTile.TileType.Exterior;
                default:
                    return BackgroundTile.TileType.Exterior;
            }
        }
    }
}
