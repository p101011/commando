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
            Foyer, LivingRoom, Bathroom, Kitchen, Office
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
                if (poi.Type == PointOfInterest.PoIType.Door) AvailableDoors.Add(poi);
            }
        }
        
        public Room(RoomType type, int numDoors) : this(RoomTemplate.GetRoomTemplate(type, numDoors, true)){}

        public void Rotate(int angle) 
        {
            float radians = Mathf.Deg2Rad * angle;
            BoundingPolygon.Rotate(angle);
            foreach (PointOfInterest poi in KeyPoints) {
                Vector3 poiC = poi.Coordinates;
                poiC.x = Mathf.Cos(radians) * (poiC.x - Coordinates.x) - Mathf.Sin(radians) * (poiC.y - Coordinates.y) +
                           Coordinates.x;
                poiC.y = Mathf.Sin(radians) * (poiC.x - Coordinates.x) - Mathf.Cos(radians) * (poiC.y - Coordinates.y) +
                           Coordinates.y;
            }
            foreach (PointOfInterest door in AvailableDoors) {
                Vector3 poiC = door.Coordinates;
                poiC.x = Mathf.Cos(radians) * (poiC.x - Coordinates.x) - Mathf.Sin(radians) * (poiC.y - Coordinates.y) +
                         Coordinates.x;
                poiC.y = Mathf.Sin(radians) * (poiC.x - Coordinates.x) - Mathf.Cos(radians) * (poiC.y - Coordinates.y) +
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
