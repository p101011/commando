using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration {
    public class WallData : MonoBehaviour
    {

        public enum WallType
        {
            Interior,
            Exterior,
            Undefined
        };

        public WallType Type { get; set;  }
        public int BuildingId { get; set; }
        public int RoomId { get; set; }

        public WallData(WallType type, int buildingId, int roomId)
        {
            Type = type;
            BuildingId = buildingId;
            RoomId = roomId;
        }
    }
}
