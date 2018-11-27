using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.LevelGeneration {
    public class Tile : Location {

        public enum TileType {
            Exterior,
            NoWall
        }

        public TileType Type;
        public int[] Walls = new int [4];

        public Tile(Coordinates c)
        {
            Coordinates = c;
            Type = TileType.Exterior;
        }

        public bool HasWall(Coordinates.Cardinality facing)
        {
            switch (facing)
            {
                case Coordinates.Cardinality.North:
                    return Walls[0] == 1;
                case Coordinates.Cardinality.East:
                    return Walls[1] == 1;
                case Coordinates.Cardinality.West:
                    return Walls[3] == 1;
                case Coordinates.Cardinality.South:
                    return Walls[2] == 1;
                default:
                    throw new ArgumentOutOfRangeException("facing", facing, null);
            }
        }

        public bool HasDoor(Coordinates.Cardinality facing) {
            switch (facing) {
                case Coordinates.Cardinality.North:
                    return Walls[0] > 1;
                case Coordinates.Cardinality.East:
                    return Walls[1] > 1;
                case Coordinates.Cardinality.West:
                    return Walls[3] > 1;
                case Coordinates.Cardinality.South:
                    return Walls[2] > 1;
                default:
                    throw new ArgumentOutOfRangeException("facing", facing, null);
            }
        }

        public void OpenDoor()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Walls[i] <= 1) continue;
                Walls[i] = 3;
                break;
            }
        }
    }
}
