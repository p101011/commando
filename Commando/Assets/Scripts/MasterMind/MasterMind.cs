﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Actors;
using Assets.Scripts.MasterMind.Goals;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = Assets.Scripts.Helpers.Logger;

namespace Assets.Scripts.MasterMind
{

    //handles decomposing and prioritizing goals
    public class MasterMind
    {

        public int Team;

        public ActorCoordinator Troops;
        public List<Goal> ActiveGoals = new List<Goal>();
        public List<Goal> QueuedGoals = new List<Goal>();

        public MasterMind(int team, IList<Actor> actors, GameObject[] troopObjects)
        {
            Team = team;
            Troops = new ActorCoordinator(actors, troopObjects);
        }

        public void AddGoal(Goal goal)
        {
            DecomposeGoal(goal);
        }

        public void ClearedPhase(Goal phase)
        {
            bool canClearPhase = true;
            while (canClearPhase) {
                ActiveGoals.Remove(phase);
                if (phase.Parent != null)
                {
                    phase.Parent.CompletePhase(phase);
                    if (phase.Parent.Phases.Count == 0)
                    {
                        phase = phase.Parent;
                    }
                    else canClearPhase = false;
                }
                else canClearPhase = false;
            }
            PrioritizeGoals(true);
        }

        public int[] EvaluateGoal(Goal goal)
        {
            int[] output = new int[2];
            switch (goal.Type)
            {
                case Goal.GoalType.SecureBuilding:
                    output[0] = UberMind.NumTroops;
                    output[1] = 0;
                    break;
                case Goal.GoalType.SecureRoom:
                    output[0] = 3;
                    output[1] = 1;
                    break;
                case Goal.GoalType.Breach:
                    output[0] = 2;
                    output[1] = 2;
                    break;
                case Goal.GoalType.MoveWaypoint:
                    output[0] = 1;
                    output[1] = 3;
                    break;
                case Goal.GoalType.Move:
                    output[0] = 1;
                    output[1] = 4;
                    break;
                case Goal.GoalType.Shoot:
                    output[0] = 1;
                    output[1] = 10;
                    break;
                case Goal.GoalType.OpenDoor:
                    output[0] = 3;
                    output[1] = 5;
                    break;
                case Goal.GoalType.Wait:
                    output[0] = 1;
                    output[1] = 8;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return output;
        }

        public void DecomposeGoal(Goal goal)
        {
            int[] evalResults = EvaluateGoal(goal);
            goal.AssessGoal(threat: evalResults[0], priority: evalResults[1]);
            QueuedGoals.Add(goal);

            List<Goal> subGoals = new List<Goal>();
            switch (goal.Type)
            {
                case Goal.GoalType.SecureBuilding:
                    return;
                case Goal.GoalType.SecureRoom:
                    return;
                case Goal.GoalType.Breach:
                    return;
                case Goal.GoalType.MoveWaypoint:
                    return;
                case Goal.GoalType.Move:
                    return;
                case Goal.GoalType.Shoot:
                    return;
                case Goal.GoalType.OpenDoor:
                    return;
                case Goal.GoalType.Wait:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (Goal subGoal in subGoals)
            {
                goal.Phases.Add(subGoal);
                DecomposeGoal(subGoal);
            }
        }

        public void PrioritizeGoals(bool shouldPrioritize)
        {
            // decide which goals should be completed first
            // need to take into account relative priority, estimated duration, actor cost
            // example: two tasks with priority 3 which take 30 seconds for 2 soldiers each
            // compared to one task with priority 4 which takes 45 seconds for 3 soldiers

            while (Troops.IdleTroops.Count > 0 && QueuedGoals.Count > 0)
            {
                // high priority first
                QueuedGoals = new List<Goal>(QueuedGoals.OrderBy(x => -x.Priority));
                Goal currentGoal = QueuedGoals[0];
                if (shouldPrioritize || currentGoal.Priority > 10)
                {
                    foreach (Goal nextGoal in QueuedGoals)
                    {
                        int priorityDifference = currentGoal.Priority - nextGoal.Priority;
                        int neededTroops = Troops.IdleTroops.Count - currentGoal.EstimatedThreat;
                        if (priorityDifference + neededTroops < 0 && nextGoal.IsAction)
                        {
                            currentGoal = nextGoal;
                        }
                    }

                    Troops.GiveGoal(currentGoal);
                    QueuedGoals.AddRange(Troops.PostponedGoals);
                    Troops.PostponedGoals = new List<Goal>();
                    ActiveGoals.Add(currentGoal);
                    QueuedGoals.Remove(currentGoal);
                    Debug.Log($"Controller {Team} is assigning {currentGoal} to the troops");
                }
                else break;
            }
        }

        public void UpdateActorState(float deltaTime)
        {
            Troops.UpdateState(deltaTime);
            foreach (Goal g in Troops.CompletedGoals)
            {
                ClearedPhase(g);
            }
        }

        public string FormatStatus()
        {
            string output = "";
            output +=
                $"Controller {Team} controls ({Troops.IdleTroops.Count}) {Troops.LivingTroops}/{Troops.StartingTroops} troops\n";
            output += $"Controller {Team} has {ActiveGoals.Count}/{QueuedGoals.Count} goals";
            Logger.LogGoals(ActiveGoals);
            Logger.LogGoals(QueuedGoals);
            return output;
        }
    }
}