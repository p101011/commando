using System.Collections.Generic;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Actors;
using Assets.Scripts.MasterMind.Goals;
using UnityEngine;


namespace Assets.Scripts.MasterMind
{
    public class UberMind : MonoBehaviour
    {

        public static int NumTeams = 1;
        public static int NumTroops = 4;
        private int _prioritizeLock = 1;
        public Building Building;
        public List<MasterMind> Controllers = new List<MasterMind>();
        public GameObject[][] TroopContainer;

        public Vector3[] CommandoSpawnPoints = { new Vector3(0, 0), new Vector3(0, 1),
                                                     new Vector3( 1, 0), new Vector3(1, 1) };
        public Vector3[] RebelSpawnPoints = { new Vector3(10, 10), new Vector3(10, 11),
                                                  new Vector3( 11, 10), new Vector3(11, 11) };

        public GameObject[] CommandoPrefabs;
        public GameObject[] RebelPrefabs;

        public void Start()
        {

            SetupTroopContainer();

            Building = GameObject.Find("GameManager").GetComponent<HouseBuilder>().Buildings[0];
            for (int i = 0; i < NumTeams; i++)
            {
                List<Actor> teamActors = new List<Actor>();
                for (int j = 0; j < NumTroops; j++) {
                    Vector3 position = GetSpawnPointVector(j, CommandoSpawnPoints);
                    GameObject actorGameObject = Instantiate(CommandoPrefabs[0], position, Quaternion.identity);
                    Actor newActor = new Actor(position, actorGameObject);
                    TroopContainer[i][j] = actorGameObject;
                    teamActors.Add(newActor);
                }
                MasterMind controller = new MasterMind(i, teamActors, TroopContainer[i]);
                //                Goal teamObjective = new Goal(Goal.GoalType.SecureBuilding, new [] {Building.Coordinates}, null);
                //                controller.AddGoal(teamObjective);
                Vector3 entranceCoordinates = new Vector3((float)GameVariables.XRes / 2, (float)GameVariables.YRes / 2);
                Goal mockObjective = new Goal(Goal.GoalType.Move, new [] {new Vector3((float) (75 * 4.8), (float) (90 * 4.8)) + entranceCoordinates}, null);
                controller.AddGoal(mockObjective);
                Debug.Log(controller.FormatStatus());
                Controllers.Add(controller);
            }

        }

        public void Update()
        {
            foreach (MasterMind controller in Controllers) 
            {
                _prioritizeLock %= 5; // we wait 5 cycles before prioritizing most goals (barring some very important ones) to allow multiple commands to queue
                controller.PrioritizeGoals(shouldPrioritize: _prioritizeLock == 0);
                controller.UpdateActorState(Time.deltaTime);
                _prioritizeLock++;
            }
        }

        private static Vector3 GetSpawnPointVector(int i, IList<Vector3> options) {
            i %= options.Count;
            Vector3 selection = options[i];
            return selection;
        }

        private void SetupTroopContainer()
        {
            TroopContainer = new GameObject[NumTeams][];
            for (int i = 0; i < NumTeams; i++)
            {
                TroopContainer[i] = new GameObject[NumTroops];
            }
        }

    }
}
