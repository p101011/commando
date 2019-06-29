using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using Assets.Scripts.LevelGeneration;
using Assets.Scripts.MasterMind.Actors;
using Assets.Scripts.MasterMind.Goals;
using UnityEditor;
using UnityEngine;


namespace Assets.Scripts.MasterMind
{
    public class UberMind : MonoBehaviour
    {

        public static int NumTeams = 1;
        public static int NumTroops = 1;
        private int _prioritizeLock = 1;
        public Building Building;
        public List<MasterMind> Controllers = new List<MasterMind>();
        public GameObject[][] TroopContainer;

        public Vector3[] CommandoSpawnPoints;
        public Vector3[] RebelSpawnPoints = { new Vector3(10, 10), new Vector3(10, 11),
                                                  new Vector3( 11, 10), new Vector3(11, 11) };

        public GameObject[] CommandoPrefabs;
        public GameObject[] RebelPrefabs;

        public void Start()
        {

            SetupTroopContainer();
            Building = GameObject.Find("GameManager").GetComponent<HouseBuilder>().Buildings[0];
            GenerateSpawnPoints();
            for (int i = 0; i < NumTeams; i++)
            {
                List<Actor> teamActors = new List<Actor>();
                for (int j = 0; j < NumTroops; j++) {
                    Vector3 position = GetSpawnPointVector(j, CommandoSpawnPoints);
                    GameObject actorGameObject = Instantiate(CommandoPrefabs[0], position, Quaternion.identity);
                    Actor newActor = new Actor(position, actorGameObject, j);
                    TroopContainer[i][j] = actorGameObject;
                    teamActors.Add(newActor);
                }
                MasterMind controller = new MasterMind(i, teamActors);
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
                controller.Update();
                _prioritizeLock++;
            }
        }

        private void GenerateSpawnPoints()
        {
            // this method will figure out where our intrepid warriors will start
            // commandos will start on some edge of the map
            List<string> spawnOptions = new List<string> {"North", "East", "South", "West"};
            string spawnLocation = spawnOptions.OrderBy(el => GUID.Generate()).First();
            const int borderOffset = 100;
            Vector3 basePoint;
            Vector3 troopSpacingX = new Vector3(40, 0);
            Vector3 troopSpacingY = new Vector3(0, 40);
            CommandoSpawnPoints = new Vector3[NumTroops];
            switch (spawnLocation)
            {
                case "North":
                {
                    basePoint = new Vector3(GameVariables.XRes / 2f, GameVariables.YRes - borderOffset);
                    if (NumTroops % 1 == 1) CommandoSpawnPoints[NumTroops - 1] = basePoint + troopSpacingY;
                    for (int i = 0; i < NumTroops - 1; i += 2) 
                    {
                        CommandoSpawnPoints[i] = basePoint + troopSpacingX - troopSpacingY * i;
                        CommandoSpawnPoints[i + 1] = basePoint - troopSpacingX - troopSpacingY * i;
                    }
                    break;
                }
                case "East":
                {
                    basePoint = new Vector3(GameVariables.XRes - borderOffset, GameVariables.YRes / 2f);
                    if (NumTroops % 1 == 1) CommandoSpawnPoints[NumTroops - 1] = basePoint + troopSpacingX;
                    for (int i = 0; i < NumTroops - 1; i += 2)
                    {
                        CommandoSpawnPoints[i] = basePoint + troopSpacingY - troopSpacingX * i;
                        CommandoSpawnPoints[i + 1] = basePoint - troopSpacingY - troopSpacingX * i;
                    }
                    break;
                }
                case "South":
                {
                    basePoint = new Vector3(GameVariables.XRes / 2f, borderOffset);
                    if (NumTroops % 1 == 1) CommandoSpawnPoints[NumTroops - 1] = basePoint - troopSpacingY;
                    for (int i = 0; i < NumTroops - 1; i += 2)
                    {
                        CommandoSpawnPoints[i] = basePoint + troopSpacingX + troopSpacingY * i;
                        CommandoSpawnPoints[i + 1] = basePoint - troopSpacingX + troopSpacingY * i;
                    }
                    break;
                }
                default:
                {
                    basePoint = new Vector3(borderOffset, GameVariables.YRes / 2f);
                    if (NumTroops % 1 == 1) CommandoSpawnPoints[NumTroops - 1] = basePoint - troopSpacingX;
                    for (int i = 0; i < NumTroops - 1; i += 2)
                    {
                        CommandoSpawnPoints[i] = basePoint + troopSpacingY + troopSpacingX * i;
                        CommandoSpawnPoints[i + 1] = basePoint - troopSpacingY + troopSpacingX * i;
                    }
                    break;
                }
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
