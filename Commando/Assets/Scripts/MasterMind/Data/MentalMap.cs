
using System;
using Assets.Scripts.Helpers;
using Vector2 = UnityEngine.Vector2;

namespace Assets.Scripts.MasterMind.Data {

    // represents a mastermind's knowledge of the terrain
    // used primarily for pathfinding
    internal class MentalMap
    {

        private readonly float[][] _grid;

        public MentalMap()
        {
            int gridXRes = GameVariables.XRes / GameVariables.MentalMapResolution;
            int gridYRes = GameVariables.YRes / GameVariables.MentalMapResolution;
            _grid = new float[gridXRes][];
            for (int i = 0; i < gridXRes; i++)
            {
                _grid[i] = new float[gridYRes];
                for (int j = 0; j < gridYRes; j++)
                {
                    _grid[i][j] = 1.0f;
                }
            }
        }

        public void WeightPoint(Vector2 point, float weight)
        {
            int xCoord = (int) point.x / GameVariables.MentalMapResolution;
            int yCoord = (int) point.y / GameVariables.MentalMapResolution;
            _grid[xCoord][yCoord] = weight;
        }
    }
}
