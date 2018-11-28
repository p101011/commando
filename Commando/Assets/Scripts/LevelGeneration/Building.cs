using System;
using System.Collections.Generic;
using Assets.Scripts.Geometry;
using Assets.Scripts.Helpers;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.LevelGeneration {


    public class Building : Location 
    {
        public List<Room> Rooms;
        public int Id;
        public List<Vector3> EntranceCoordinates;
        public Polygon BoundingPolygon;

        public Building(int id, Vector3 doorCoordinates)
        {
            Rooms = new List<Room>();
            Id = id;
            EntranceCoordinates = new List<Vector3>{doorCoordinates};

            CreateRooms();
        }

        private void CreateRooms()
        {

            Room currentRoom = new Room(Room.RoomType.Foyer, 0);
            currentRoom.Translate(EntranceCoordinates[0]);
            Rooms.Add(currentRoom);
            List<Room> roomsToBuild = new List<Room>{currentRoom};
            BoundingPolygon = new Polygon(currentRoom.BoundingPolygon);
            while (roomsToBuild.Count > 0)
            {
                currentRoom = roomsToBuild[0];
                roomsToBuild.RemoveAt(0);

                while (currentRoom.AvailableDoors.Count > 0)
                {
                    PointOfInterest door = currentRoom.AvailableDoors[0];
                    // get random type of room excluding foyers
                    Room.RoomType type = (Room.RoomType) Random.Range(1, Enum.GetValues(typeof(Room.RoomType)).Length);
                    currentRoom.AvailableDoors.RemoveAt(0);
                    bool exhaustedTemplates = false;
                    bool templateValid = false;
                    bool firstTry = true;
                    RoomTemplate template = null;
                    while (!templateValid && !exhaustedTemplates)
                    {
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        // bad ReSharper
                        template = RoomTemplate.GetRoomTemplate(type, 1, firstTry);
                        firstTry = false;
                        if (template == null)
                        {
                            Debug.Log("Failed to find an appropriate RoomTemplate");
                            exhaustedTemplates = true;
                            EntranceCoordinates.Add(door.Coordinates);
                        }
                        else 
                        {
                            int newRoomDoorIndex = 0;
                            Room tempRoom = new Room(template);
                            while (newRoomDoorIndex < tempRoom.AvailableDoors.Count && !templateValid)
                            {
                                int doorFacing;
                                switch (door.Direction) {
                                    case PointOfInterest.Facing.North:
                                        doorFacing = (int) PointOfInterest.Facing.South;
                                        break;
                                    case PointOfInterest.Facing.East:
                                        doorFacing = (int)PointOfInterest.Facing.West;
                                        break;
                                    case PointOfInterest.Facing.South:
                                        doorFacing = (int)PointOfInterest.Facing.North;
                                        break;
                                    case PointOfInterest.Facing.West:
                                        doorFacing = (int)PointOfInterest.Facing.East;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                int rotation = (90 * doorFacing) + 180;
                                tempRoom.Rotate(rotation);
                                PointOfInterest tempDoor = tempRoom.AvailableDoors[newRoomDoorIndex];
                                Vector3 translation = tempDoor.Coordinates - door.Coordinates;
                                tempRoom.Translate(translation);
                                if (!BoundingPolygon.Intersects(tempRoom.BoundingPolygon))
                                    templateValid = true;
                                else
                                {
                                    newRoomDoorIndex ++;
                                }
                            }
                        }
                    }

                    if (!templateValid) continue;

                    //todo: need to modify template coords to reflect actual position/rotation of room
                    Room newRoom = new Room(template);
                    currentRoom.AdjacentRooms.Add(newRoom);
                    Rooms.Add(newRoom);
                    roomsToBuild.RemoveAt(0);
                    BoundingPolygon = new Polygon(BoundingPolygon, newRoom.BoundingPolygon);
                }
            }
        }
    }
}
