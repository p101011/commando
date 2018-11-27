using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.MasterMind.Goals;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Helpers {
    public class Logger {

        public static void LogGoals(IEnumerable<Goal> goals)
        {
            foreach (Goal goal in goals)
            {
                Debug.Log(goal.FormatGoal());
            }
        }
    }
}
