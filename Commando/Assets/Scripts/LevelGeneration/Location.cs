using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.LevelGeneration {

    public class Location {

        public Coordinates Coordinates = new Coordinates(0, 0);

        public Location()
        {
        }

        public Location(Coordinates c)
        {
            Coordinates = c;
        }
    }
}
