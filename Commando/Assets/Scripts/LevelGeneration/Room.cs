using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room {


	public Coordinates Coordinates = new Coordinates(0, 0);                 // The y coordinate of the lower left tile of the room.
    public int RoomWidth;                     // How many tiles wide the room is.
    public int RoomHeight;                    // How many tiles high the room is.
    public List<Room> AdjacentRooms;

    private readonly int[] _nextOpenSpaces = {0, 0, 0, 0};  // {N, E, S, W} offset from left tile

    public Room(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        // Set a random width and height.
        RoomWidth = widthRange.Random;
        RoomHeight = heightRange.Random;
        Coordinates.X = columns / 2;
        Coordinates.Y = rows / 2;
        AdjacentRooms = new List<Room>();
    }

    public Room(IntRange widthRange, IntRange heightRange, Coordinates anchor)
    {
        RoomWidth = widthRange.Random;
        RoomHeight = heightRange.Random;
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
        int xPos = Coordinates.X + xOffset;
        int yPos = Coordinates.Y + yOffset;
        Coordinates coords = new Coordinates(xPos, yPos);
        Room builtRoom = new Room(widthRange, heightRange, coords);
        int addedBlocker = i % 2 == 0 ? builtRoom.RoomWidth : builtRoom.RoomHeight;
        _nextOpenSpaces[i] += addedBlocker;
        AdjacentRooms.Add(builtRoom);
        return builtRoom;
    }

    public HouseBuilder.TileType GetTileType(int x, int y)
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
                return HouseBuilder.TileType.NoWall;
            case 1:
                return HouseBuilder.TileType.Exterior;
            case 2:
                return HouseBuilder.TileType.Exterior;
            case 3:
                return HouseBuilder.TileType.Exterior;
            default:
                return HouseBuilder.TileType.Exterior;
        }
    }
}
