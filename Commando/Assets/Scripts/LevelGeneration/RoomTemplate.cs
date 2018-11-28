using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Geometry;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Assets.Scripts.LevelGeneration {
    public class RoomTemplate
    {

        public Edge[] Edges;
        public List<PointOfInterest> KeyPoints;

        private Dictionary<Room.RoomType, List<int>> _hellBeast;

        public RoomTemplate(Edge[] edges, List<PointOfInterest> keyPoints)
        {
            Edges = edges;
            KeyPoints = keyPoints;
        }

        public void BuildTheBeast()
        {
            _hellBeast.Add(Room.RoomType.Foyer, List<1>);
        }


        public static RoomTemplate GetRoomTemplate(Room.RoomType type, int numDoorsConnecting, bool newTry)
        {
            throw new NotImplementedException();
        }
    }
}
