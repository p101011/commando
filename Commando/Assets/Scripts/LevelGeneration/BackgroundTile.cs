using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.LevelGeneration {
    public class BackgroundTile : Location {

        public enum TileType {
            Exterior,
            Interior
        }

        public TileType Type;

        public BackgroundTile()
        {
            Type = TileType.Exterior;
        }
    }
}
