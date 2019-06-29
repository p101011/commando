using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.MasterMind.Data;
using Assets.Scripts.MasterMind.Goals;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.MasterMind.Actors
{
    public class Actor
    {

        public Goal CurrentGoal;
        public System.Collections.Generic.List<Goal> NextGoals = new System.Collections.Generic.List<Goal>();
        public bool IsIdle = true;
        public GameObject Instance;
        public ActorCoordinator Owner;
        public int Id;

        public Vector3 Position;
        public Quaternion Facing = Quaternion.identity;
        private Vector3 _goalPositionVector;
        private Quaternion _goalFacing = Quaternion.identity;

        private BoxCollider2D _ownCollider;

        private static readonly Vector3 FoVAngleOffset = Quaternion.AngleAxis(GameVariables.FieldOfView / 2f, Vector3.forward) * Vector3.right;
        private static readonly Quaternion FoVAngleStep = Quaternion.AngleAxis(GameVariables.FieldOfView / GameVariables.DegreesPerLoSRay, Vector3.forward);

        public Actor(Vector3 startPosition, GameObject i, int id)
        {
            Position = startPosition;
            Instance = i;
            _ownCollider = i.GetComponent<BoxCollider2D>();
            Id = id;
        }

        public ActorSensorData UpdateState()
        {
            Position = Instance.transform.localPosition;
            Facing = Instance.transform.rotation;
            VisualCone visualTargets = CheckSight();
            AudioSensorResult[] audioTargets = CheckAudio();
            return new ActorSensorData(this, visualTargets, audioTargets);
        }

        private VisualCone CheckSight()
        {
            Vector3 castAngle = Facing * Vector3.right + FoVAngleOffset;
            RaycastHit2D[] results = new RaycastHit2D[0];
            for (int i = 0; i < GameVariables.FieldOfView / GameVariables.DegreesPerLoSRay; i++)
            {
                RaycastHit2D[] traceHits = RayTracer.LoSRayCast(Position, castAngle);
                traceHits = CullOcclusion(traceHits);
                Array.Resize(ref results, results.Length + traceHits.Length);
                traceHits.CopyTo(results, results.Length - traceHits.Length);
                castAngle = FoVAngleStep * castAngle;
            }

            return BuildCone(results);
        }

        private AudioSensorResult[] CheckAudio()
        {
            return null;
        }

        public override string ToString()
        {
            return $"Actor's current goal is {CurrentGoal} and actor has {NextGoals.Count} other goals";
        }

        private RaycastHit2D[] CullOcclusion(IReadOnlyList<RaycastHit2D> raw)
        {
            List<int> validIndices = new List<int>();
            for (int index = 0; index < raw.Count; index++)
            {
                if (raw[index].collider.Equals(_ownCollider)) continue;
                validIndices.Add(index);
                if (raw[index].transform.gameObject.layer == LayerMask.NameToLayer("OpaqueLoSTarget")) break;
            }
            RaycastHit2D[] output = new RaycastHit2D[validIndices.Count];
            for (int i = 0; i < validIndices.Count; i++)
            {
                output[i] = raw[validIndices[i]];
            }

            return output;
        }

        private VisualCone BuildCone(RaycastHit2D[] hits)
        {
            // first hit is guaranteed to be right-most point
            // last hit is left-most

            VisualCone output = new VisualCone(Position, hits[0].point, hits[hits.Length - 1].point);

            for (int i = 1; i < hits.Length - 1; i++)
            {

            }

            return output;
        }
    }
}
