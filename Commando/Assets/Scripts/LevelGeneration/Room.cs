using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Geometry;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration
{
    public class Room : Location
    {
        public enum RoomType
        {
            Foyer, LivingRoom, Bathroom, Kitchen, Bedroom
        }

        public Polygon BoundingPolygon;
        public List<Room> AdjacentRooms = new List<Room>();
        public List<PointOfInterest> KeyPoints;
        public List<PointOfInterest> AvailableDoors = new List<PointOfInterest>();  // used for building rooms

        public Room(RoomTemplate template)
        {
            BoundingPolygon = new Polygon(new List<Edge>(template.Edges));
            KeyPoints = template.KeyPoints;

            if (!BoundingPolygon.Vertices.Any() || KeyPoints.Count == 0) Debug.Log("Received a bad room template!");

            Coordinates = BoundingPolygon.Center;

            foreach (PointOfInterest poi in KeyPoints) {
                if (poi.Type == PointOfInterest.PoIType.Door && poi.Available) AvailableDoors.Add(poi);
            }
        }
        
        public Room(RoomType type, int numDoors) : this(RoomTemplate.GetRoomTemplate(type, numDoors, true)){}

        public void Rotate(int angle) 
        {
            float radians = Mathf.Deg2Rad * angle;
            BoundingPolygon.Rotate(angle);
            foreach (PointOfInterest poi in KeyPoints) {
                poi.Coordinates.x = Mathf.Cos(radians) * (poi.Coordinates.x - Coordinates.x) - Mathf.Sin(radians) * (poi.Coordinates.y - Coordinates.y) +
                           Coordinates.x;
                poi.Coordinates.y = Mathf.Sin(radians) * (poi.Coordinates.x - Coordinates.x) - Mathf.Cos(radians) * (poi.Coordinates.y - Coordinates.y) +
                           Coordinates.y;
            }
            foreach (PointOfInterest door in AvailableDoors) {
                door.Coordinates.x = Mathf.Cos(radians) * (door.Coordinates.x - Coordinates.x) - Mathf.Sin(radians) * (door.Coordinates.y - Coordinates.y) +
                         Coordinates.x;
                door.Coordinates.y = Mathf.Sin(radians) * (door.Coordinates.x - Coordinates.x) + Mathf.Cos(radians) * (door.Coordinates.y - Coordinates.y) +
                         Coordinates.y;
            }
        }

        public void Translate(Vector3 translation)
        {
            BoundingPolygon.Translate(translation);
            foreach (PointOfInterest poi in KeyPoints)
            {
                poi.Coordinates += translation;
            }
            foreach (PointOfInterest door in AvailableDoors) {
                door.Coordinates += translation;
            }
        }
    }
}
