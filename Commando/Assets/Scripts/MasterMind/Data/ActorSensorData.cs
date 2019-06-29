using System;
using Assets.Scripts.MasterMind.Actors;
using UnityEngine;

namespace Assets.Scripts.MasterMind.Data {

    // this is used to inform master mind of two things: what actor sees/hears
    // and any actions the actor took

    public class ActorSensorData
    {
        public Actor Owner { get; }
        public VisualCone SightCone { get; }
        public AudioSensorResult[] AudioResults { get; }
        public DateTime Time { get; }

        public ActorSensorData(Actor owner, VisualCone sightResults, AudioSensorResult[] audioResults)
        {
            Owner = owner;
            SightCone = sightResults;
            AudioResults = audioResults;
            Time = DateTime.Now;
        }

    }
}
