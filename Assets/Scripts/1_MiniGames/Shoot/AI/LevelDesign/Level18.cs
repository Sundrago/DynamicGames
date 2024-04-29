using UnityEngine;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level18 : IAIBehavior
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
                StageLevel = 18,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(300),
                    new CreateMeteor(5, Random.Range(200,1000)),
                    new SpawnEnemyInLineY(15, -0.9f),
                    new SpawnEnemyInLineY(15, 0.7f)
                },
                NumberOfRandomTasksToPerform = 5,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(4, Random.Range(200,1000)),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInCircle(1f, Random.Range(5, 18)),
                        new SpawnEnemyInCircle(1.35f, Random.Range(8, 25)),
                        new Delay(2000),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 8),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInSpiral(0.5f, 2f, 30, 1.5f, 30),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new Delay(1000),
                        new SpawnEnemyInLineY(16, -0.9f),
                        new Delay(1000),
                        new SpawnEnemyInLineY(16, -0.85f),
                        new Delay(2000),
                    },
                },
                PostTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Close),
                    new Delay(500)
                },
                PostRoutine = new AIRoutine[]
                {
                    new(AIRoutineType.CreateEnemyInCircle, 4700, 7, 20),
                    new(AIRoutineType.CreateEnemyRandomPos, 600, 1, 4),
                    new(AIRoutineType.CreateMeteor, 500, 1, 2, 0.25f),
                    new(AIRoutineType.CreateEnemyInLine, 3000, 4, 12, 0.45f)
                }
            };
        }
    }
}