using UnityEngine;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level11 : IAIBehavior
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
                StageLevel = 11,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(1000),
                    new CreateMeteor(2, Random.Range(500,1000)),
                    new SpawnEnemyInLineY(15)
                },
                NumberOfRandomTasksToPerform = 3,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(3, Random.Range(400,1000)),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInCircle(1f, Random.Range(5, 15)),
                        new SpawnEnemyInCircle(1.35f, Random.Range(8, 20)),
                        new Delay(3200),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 5),
                        new Delay(3000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInSpiral(0.5f, 2f, 30, 1.3f, 30),
                        new Delay(2000)
                    },
                    new IAITaskParameter[]
                    {
                        new Delay(2000),
                        new SpawnEnemyInLineY(10, -0.9f),
                        new Delay(1500),
                        new SpawnEnemyInLineY(10, -0.85f),
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
                    new(AIRoutineType.CreateEnemyInCircle, 8000, 3, 12),
                    new(AIRoutineType.CreateEnemyRandomPos, 1800, 1, 4),
                    new(AIRoutineType.CreateMeteor, 2400, 0, 2, 0.5f),
                    new(AIRoutineType.CreateEnemyInLine, 6500, 5, 12, 0.45f)
                }
            };
        }
    }
}