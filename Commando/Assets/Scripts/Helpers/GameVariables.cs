using UnityEngine;

namespace Assets.Scripts.Helpers {
    public static class GameVariables
    {

        public static int XRes = 2560;
        public static int YRes = 1440;

        public static float ActorRotateSpeed = 45;
        public static float ActorMoveSpeed = 50;
        public static double ActorReach = 0.3;

        public static double Tolerance = float.Epsilon;

        public static bool Debug = true;

        // AI VARIABLES //

            // Sensor Variables //
        public static int FieldOfView = 190;
        public static float DegreesPerLoSRay = 19f;

            // Pathfinding Variables //
        public static int MentalMapResolution = 10;
    }
}
