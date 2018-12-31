using System;
using System.Collections.Generic;
using Assets.Scripts.Geometry;
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


        private readonly List<Room> _roomsToBuild = new List<Room>();

        public Building(int id, Vector3 doorCoordinates, bool custom=false)
        {
            Rooms = new List<Room>();
            Id = id;
            EntranceCoordinates = new List<Vector3>{doorCoordinates};

            if (!custom) CreateRooms();
        }

        private void CreateRooms()
        {

            Room currentRoom = new Room(Room.RoomType.Foyer, 0);
            currentRoom.Translate(EntranceCoordinates[0]);
            Rooms.Add(currentRoom);
            _roomsToBuild.Add(currentRoom);
            BoundingPolygon = new Polygon(currentRoom.BoundingPolygon);
            while (_roomsToBuild.Count > 0)
            {
                currentRoom = _roomsToBuild[0];
                BuildNewRoom(currentRoom);
                _roomsToBuild.RemoveAt(0);
            }
        }

        private void BuildNewRoom(Room seedRoom)
        {
            while (seedRoom.AvailableDoors.Count > 0)
            {
                int randIndex = Random.Range(0, seedRoom.AvailableDoors.Count);
                PointOfInterest door = seedRoom.AvailableDoors[randIndex];
                // get random type of room excluding foyers
                Room.RoomType type = (Room.RoomType)Random.Range(1, Enum.GetValues(typeof(Room.RoomType)).Length);
                seedRoom.AvailableDoors.RemoveAt(randIndex);
                bool exhaustedTemplates = false;
                bool templateValid = false;
                bool firstTry = true;
                Room newRoom = null;
                while (!templateValid && !exhaustedTemplates) {
                    RoomTemplate template = RoomTemplate.GetRoomTemplate(type, 1, firstTry);
                    firstTry = false;
                    if (template == null) {
                        Debug.Log("Failed to find an appropriate RoomTemplate");
                        exhaustedTemplates = true;
                        EntranceCoordinates.Add(door.Coordinates);
                    }
                    else {
                        newRoom = new Room(template);
                        templateValid = TryFitNewRoom(newRoom, door);
                    }
                }

                if (!templateValid) continue;

                seedRoom.AdjacentRooms.Add(newRoom);
                Rooms.Add(newRoom);
                _roomsToBuild.Add(newRoom);
                BoundingPolygon = new Polygon(BoundingPolygon, newRoom.BoundingPolygon);
            }
        }

        private bool TryFitNewRoom(Room testRoom, PointOfInterest door)
        {
            int newRoomDoorIndex = 0;
            while (newRoomDoorIndex < testRoom.AvailableDoors.Count) {
                PointOfInterest tempDoor = testRoom.AvailableDoors[newRoomDoorIndex];
                int rotation = (90 * (int) door.Direction + 90 * (int) tempDoor.Direction) % 360;
                testRoom.Rotate(rotation);
                Vector3 translation = tempDoor.Coordinates - door.Coordinates;
                testRoom.Translate(translation);
                if (!BoundingPolygon.Intersects(testRoom.BoundingPolygon))
                    return true;
                // room didn't fit this way, undo our changes
                testRoom.Rotate(-rotation);
                testRoom.Translate(-translation);
                newRoomDoorIndex++;
            }

            return false;
        }
    }
}
