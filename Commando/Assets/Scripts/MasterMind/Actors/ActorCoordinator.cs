using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Goals;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Actors {

    //handles assigning decomposed actions to troops
    public class ActorCoordinator
    {

        public int LivingTroops;
        public int StartingTroops;
        public List<Actor> IdleTroops;
        public List<Actor> Troops;
        public GameObject[] TroopObjects;
        public List<Goal> CompletedGoals = new List<Goal>();


        public ActorCoordinator(IList<Actor> actors, GameObject[] troopObjects)
        {
            LivingTroops = StartingTroops = actors.Count;
            TroopObjects = troopObjects;
            Troops = new List<Actor>(StartingTroops);
            IdleTroops = new List<Actor>(StartingTroops);
            for (int i = 0; i < StartingTroops; i++)
            {
                IdleTroops.Add(actors[i]);
                Troops.Add(actors[i]);
                actors[i].Owner = this;
            }
        }

        public void GiveGoal(Goal newGoal)
        {
            IdleTroops[0].GiveGoal(newGoal);
            IdleTroops.RemoveAt(0);
        }

        public void UpdateState(float deltaTime)
        {
            foreach (Actor actor in Troops)
            {
                if (actor.IsIdle) continue;
                actor.CurrentGoal.EstimatedCompletion -= deltaTime;
                switch (actor.CurrentGoal.Type) {
                    case Goal.GoalType.SecureBuilding:
                        break;
                    case Goal.GoalType.SecureRoom:
                        break;
                    case Goal.GoalType.Breach:
                        break;
                    case Goal.GoalType.MoveWaypoint:
                        List<Location> path = actor.CurrentGoal.Target;
                        Goal parent = actor.CurrentGoal.Parent;
                        Goal waypoint = new Goal(Goal.GoalType.Move, new[] { path[0].Coordinates }, parent);
                        List<Vector3> restCoordinates = new List<Vector3>(path.Count - 1);
                        for (int i = 1; i < path.Count; i++) {
                            restCoordinates.Add(path[i].Coordinates);
                        }
                        Goal rest = new Goal(Goal.GoalType.Move, restCoordinates, parent);
                        actor.SetGoal(waypoint);
                        actor.GiveGoal(rest);
                        break;
                    case Goal.GoalType.Move:
                        actor.MoveStep(deltaTime);
                        break;
                    case Goal.GoalType.Shoot:
                        break;
                    case Goal.GoalType.OpenDoor:
                        BackgroundTile doorTile = actor.CurrentGoal.Target[0] as BackgroundTile;
                        float distance =
                            Vector3.Magnitude(actor.PositionVector - doorTile.Coordinates);
                        if (distance <= GameVariables.ActorReach) {
                            actor.OpenDoor();
                        } else
                        {
                            Goal oldGoal = actor.CurrentGoal;
                            actor.GiveGoal(actor.CurrentGoal);
                            actor.SetGoal(new Goal(Goal.GoalType.Move, oldGoal.Target, oldGoal.Parent));
                            oldGoal.Parent.Phases.Add(actor.CurrentGoal);
                        }
                        break;
                    case Goal.GoalType.Wait:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
