using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class Coordinates {

        public enum Cardinality {
            North, East, West, South
        }

        public double X;
        public double Y;

        public Coordinates(double xPos, double yPos)
        {
            X = xPos;
            Y = yPos;
        }

        public Coordinates Add(double x, double y)
        {
            return new Coordinates(X + x, Y + y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float) X, (float) Y);
        }
    }
}