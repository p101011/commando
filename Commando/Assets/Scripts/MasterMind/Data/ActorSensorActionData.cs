using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Data {

    // this is used to inform master mind of two things: what actor sees/hears
    // and any actions the actor took

    public class ActorSensorActionData
    {

        public Action[] Actions { get; }
        public RaycastHit2D[] RaycastResults { get; }
        public AudioSensorResult[] AudioResults { get; }
        public DateTime Time { get; }

        public ActorSensorActionData(Action[] actions, RaycastHit2D[] sightResults, AudioSensorResult[] audioResults)
        {
            Actions = actions;
            RaycastResults = sightResults;
            AudioResults = audioResults;
            Time = DateTime.Now;
        }

    }
}
