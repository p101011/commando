using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Data {

    public enum SensorType
    {
        Visual,
        Audio
    };

    // this is used to keep a 'mental map' for the mastermind

    internal class SpottingData
    {
        public DateTime Time { get; }
        public SensorType Type { get; }
        public GameObject Source { get; }
        public GameObject Target { get; }
        public Vector3 SourceLocation { get; }
        public Vector3 TargetLocation { get; }


        public SpottingData(DateTime time, SensorType type, GameObject source, GameObject target, Vector3 sourceLocation,
            Vector3 targetLocation)
        {
            Time = time;
            Type = type;
            Source = source;
            Target = target;
            TargetLocation = targetLocation;
            SourceLocation = sourceLocation;
        }
    }
}
