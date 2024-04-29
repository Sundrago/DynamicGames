using UnityEngine;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level14 : IAIBehavior
    {
        private static AIBehavior aiBehavior = null;
        public AIBehavior AIBehavior
        {
            get
            {
                if (aiBehavior == null)
                {
                    aiBehavior = GetBehavior();
                }
                return aiBehavior;
            }
        }

        private static AIBehavior GetBehavior()
        {
            return new AIBehavior()
            {
                StageLevel = 14,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(300),
                    new CreateMeteor(4, Random.Range(200,1000)),
                    new SpawnEnemyInLineY(15, -0.9f)
                },
                NumberOfRandomTasksToPerform = 4,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(3, Random.Range(300,1000)),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInCircle(1f, Random.Range(5, 18)),
                        new SpawnEnemyInCircle(1.35f, Random.Range(8, 25)),
                        new Delay(3200),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 8),
                        new Delay(2000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInSpiral(0.5f, 2f, 30, 1.5f, 30),
                        new Delay(2000)
                    },
                    new IAITaskParameter[]
                    {
                        new Delay(2000),
                        new SpawnEnemyInLineY(14, -0.9f),
                        new Delay(1500),
                        new SpawnEnemyInLineY(14, -0.85f),
                        new Delay(4000),
                    },
                },
                PostTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Close),
                    new Delay(1000)
                },
                PostRoutine = new AIRoutine[]
                {
                    new(AIRoutineType.CreateEnemyInCircle, 5500, 7, 20),
                    new(AIRoutineType.CreateEnemyRandomPos, 1000, 1, 4),
                    new(AIRoutineType.CreateMeteor, 1100, 1, 1, 0.5f),
                    new(AIRoutineType.CreateEnemyInLine, 4900, 5, 10, 0.45f)
                }
            };
        }
    }
}