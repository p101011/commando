using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.LevelGeneration {

    public class Location {

        public Vector3 Coordinates = new Vector3(0, 0);

        public Location()
        {
        }

        public Location(Vector3 c)
        {
            Coordinates = c;
        }
    }
}
