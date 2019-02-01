using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Data;
using Assets.Scripts.MasterMind.Goals;
using Boo.Lang.Environments;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.MasterMind.Actors
{
    public class Actor
    {

        public Goal CurrentGoal;
        public List<Goal> NextGoals = new List<Goal>();
        public bool IsIdle = true;
        public Vector3 Coordinates;
        public GameObject Instance;
        public ActorCoordinator Owner;

        public Vector3 Position;
        public Quaternion Facing = Quaternion.identity;
        private Vector3 _goalPositionVector;
        private Quaternion _goalFacing = Quaternion.identity;

        private static Vector3 _foVAngleOffset = 

        public Actor(Vector3 startPosition, GameObject i)
        {
            Position = startPosition;
            Instance = i;
        }

        public ActorSensorActionData UpdateState()
        {
            RaycastHit2D[] visualTargets = CheckSight();
            AudioSensorResult[] audioTargets = CheckAudio();
            Action[] reflexes = EvaluateActions();
            return new ActorSensorActionData(reflexes, visualTargets, audioTargets);
        }

        private RaycastHit2D[] CheckSight()
        {
            Vector3 startingAngle = (Facing * Vector3.right) - _foVAngleOffset;
        }

        public override string ToString()
        {
            return $"Actor's current goal is {CurrentGoal} and actor has {NextGoals.Count} other goals";
        }
    }
}
