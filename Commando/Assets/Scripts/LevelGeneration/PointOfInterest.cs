using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration {
    public class PointOfInterest : Location{

        public enum PoIType
        {
            Door, Window
        }

        public enum Facing
        {
            North, East, South, West
        }

        public PoIType Type;
        public Facing Direction;
        public bool Available;

        public PointOfInterest(PoIType type, Facing direction, bool available, Vector3 coordinates)
        {
            Type = type;
            Coordinates = coordinates;
            Direction = direction;
            Available = available;
        }

        public override string ToString()
        {
            return $"{Type} @ {Coordinates}";
        }
    }
}
