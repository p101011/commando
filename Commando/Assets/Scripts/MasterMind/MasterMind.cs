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
        private State _currentState;
        private MentalMap _map = new MentalMap();
        public List<Goal> ActiveGoals = new List<Goal>();
        public List<Goal> QueuedGoals = new List<Goal>();

        public MasterMind(int team, IEnumerable<Actor> actors)
        {
            Team = team;
            Troops = actors.ToArray();
        }

        public void AddGoal(Goal newGoal)
        {

        }

        public void Update()
        {
            _currentState = new State();
            UpdateSensors();
            EvaluateActorStates();
            PrioritizeGoals();
            AssignGoals();
        }

        private void UpdateSensors()
        {
            foreach (Actor soldier in Troops)
            {
                ActorSensorData soldierResponse = soldier.UpdateState();
                ProcessSensorData(soldierResponse);
            }
        }

        private void ProcessSensorData(ActorSensorData input)
        {
            UpdateMap(input.SightCone);
            UpdateState(input);
        }

        private void UpdateMap(VisualCone visionData)
        {
            // as index increases, hits sweep right to left 

            // remove hits which don't provide new info - e.g. hits on same wall

//            HashSet<Rigidbody2D> seenRigidBodies = new HashSet<Rigidbody2D>();
//            List<RaycastHit2D> prunedData = new List<RaycastHit2D>();
//            foreach (RaycastHit2D hit in visionData)
//            {
//                if (seenRigidBodies.Contains(hit.rigidbody)) continue;
//                seenRigidBodies.Add(hit.rigidbody);
//                prunedData.Add(hit);
//            }
        
            

        }

        private void UpdateState(ActorSensorData input)
        {

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
            return "TODO: log this state";
        }
    }
}