using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Goals
{
    public class Goal
    {

        public enum GoalType
        {
            SecureBuilding, SecureRoom, Breach, MoveWaypoint, Move, Shoot, OpenDoor, Wait
        }

        public int EstimatedThreat;
        public int Priority;
        public GoalType Type;
        public Goal Parent;
        public List<Location> Target = new List<Location>();
        public List<Goal> Phases = new List<Goal>();
        public float EstimatedCompletion = 0;
        public bool IsAction;

        public Goal(GoalType type, int priorityMod, Goal parent) {
            Type = type;
            IsAction = type == GoalType.Move || type == GoalType.Shoot || type == GoalType.OpenDoor;
            Priority = priorityMod;
            Parent = parent;
        }

        public Goal(GoalType type, IEnumerable<Coordinates> coordinates, Goal parent, int priorityMod = 0) : this (type, priorityMod, parent)
        {
            foreach (Coordinates c in coordinates)
            {
                Target.Add(new Location(c));
            }
        }

        public Goal(GoalType type, IEnumerable<Location> locations, Goal parent, int priorityMod = 0) : this(type, priorityMod, parent)
        {
            Target = new List<Location>(locations);
        }

        public void AssessGoal(int threat, int priority)
        {
            EstimatedThreat = threat;
            Priority += priority;
        }

        public void CompletePhase(Goal phase)
        {
            Phases.Remove(phase);
        }

        public string FormatGoal()
        {
            return string.Format("{0:G}: Threat={1}, Priority={2}", Type, EstimatedThreat, Priority);
        }
    }
}
