using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
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
        public Coordinates Coordinates;
        public GameObject Instance;
        public ActorCoordinator Owner;

        public Vector3 PositionVector;
        public Quaternion Facing = Quaternion.identity;
        private Vector3 _goalPositionVector;
        private Quaternion _goalFacing = Quaternion.identity;

        public Actor(Vector3 startPosition, GameObject i)
        {
            Coordinates = new Coordinates(startPosition.x, startPosition.y);
            PositionVector = startPosition;
            Instance = i;
        }

        public void  SetGoal(Goal newGoal) {
            CurrentGoal = newGoal;
            IsIdle = false;
            switch (newGoal.Type)
            {
                case Goal.GoalType.SecureBuilding:
                    break;
                case Goal.GoalType.SecureRoom:
                    break;
                case Goal.GoalType.Breach:
                    break;
                case Goal.GoalType.Shoot:
                    break;
                case Goal.GoalType.MoveWaypoint:
                case Goal.GoalType.Move:
                    _goalPositionVector = new Vector3((float) newGoal.Target[0].Coordinates.X, (float) newGoal.Target[0].Coordinates.Y);
//                    float angleBetween = Vector3.Angle(_positionVector, _goalPositionVector);
                    _goalFacing = Quaternion.FromToRotation(PositionVector, _goalPositionVector);
                    break;
                case Goal.GoalType.OpenDoor:
                    break;
                default:
                    break;
            }
        }

        public void GiveGoal(Goal newGoal)
        {
            if (CurrentGoal == null)
            {
                SetGoal(newGoal);
            }
            else
            {
                NextGoals.Add(newGoal);
            }
        }

        public void ClearGoal()
        {
            Owner.CompletedGoals.Add(CurrentGoal);
            if (NextGoals.Count > 0)
            {
                CurrentGoal = NextGoals[0];
                NextGoals.RemoveAt(0);
            }
            else
            {
                CurrentGoal = null;
                IsIdle = true;
            }
        }

        public void OpenDoor()
        {
            Tile target = CurrentGoal.Target[0] as Tile;
            Debug.Assert(target != null, "target != null");
            target.OpenDoor();
            ClearGoal();
            UnityEngine.Debug.Log("Breaching!");
        }

        public void MoveStep(float deltaTime)
        {
            if (!_goalFacing.Equals(Facing))
            {
                Facing = Quaternion.RotateTowards(Facing, _goalFacing, GameVariables.ActorRotateSpeed * deltaTime);
            }

//            Vector3 stepVector = _facing * new Vector3(0, GameSettings.ActorMoveSpeed * deltaTime, 0);
            PositionVector = Vector3.MoveTowards(PositionVector, _goalPositionVector,
                GameVariables.ActorMoveSpeed * deltaTime);
//            _positionVector += stepVector;
            Instance.transform.rotation = Facing;
            Instance.transform.position = PositionVector;
            if ((PositionVector - _goalPositionVector).sqrMagnitude < GameVariables.Tolerance * GameVariables.Tolerance)
            {
                ClearGoal();
            }
        }

        public string FormatActor()
        {
            return string.Format("Actor's current goal is {0} and actor has {1} other goals", CurrentGoal.FormatGoal(), NextGoals.Count);
        }
    }
}
