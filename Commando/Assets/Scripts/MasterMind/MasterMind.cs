using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Actors;
using Assets.Scripts.MasterMind.Data;
using Assets.Scripts.MasterMind.Goals;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = Assets.Scripts.Helpers.Logger;

namespace Assets.Scripts.MasterMind
{

    // provides strategy and coordination between actors
    public class MasterMind
    {

        public int Team;

        public Actor[] Troops;
        public List<Goal> ActiveGoals = new List<Goal>();
        public List<Goal> QueuedGoals = new List<Goal>();

        private int _idleCount;
        private int _livingCount;

        public MasterMind(int team, IEnumerable<Actor> actors)
        {
            Team = team;
            Troops = actors.ToArray();
            _idleCount = _livingCount = Troops.Length;
        }

        public void AddGoal(Goal newGoal)
        {

        }

        public void Update()
        {
            UpdateState();
            EvaluateActorStates();
            PrioritizeGoals();
            AssignGoals();
        }

        private void UpdateState()
        {
            foreach (Actor soldier in Troops)
            {
                ActorSensorActionData soldierResponse = soldier.UpdateState();
                ProcessSensorActionData(soldierResponse);
            }
        }

        private void EvaluateActorStates()
        {

        }

        private void PrioritizeGoals()
        {

        }

        private void AssignGoals()
        {

        }

        public string FormatStatus()
        {
            string output = "";
            output +=
                $"Controller {Team} controls ({_idleCount}) {_livingCount}/{Troops.Length} troops\n";
            output += $"Controller {Team} has {ActiveGoals.Count}/{QueuedGoals.Count} goals";
            Logger.LogGoals(ActiveGoals);
            Logger.LogGoals(QueuedGoals);
            return output;
        }
    }
}