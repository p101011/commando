using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration {
    public class PhysicsData : MonoBehaviour
    {

        public enum ObjectType
        {
            InteriorWall,
            ExteriorWall,
            Window,
            Room,
            Building,
            Undefined
        };

        public ObjectType Type;
        public int BuildingId;
        public int RoomId;
        public bool Opaque;
    }
}
