using UnityEngine;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level09 : IAIBehavior
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
                StageLevel = 9,
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
                },
                NumberOfRandomTasksToPerform = 2,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(3, Random.Range(400,1000)),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInCircle(1.25f, Random.Range(10, 30)),
                        new Delay(3200),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 5),
                        new Delay(3000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInSpiral(0.6f, 2f, 30, 1.3f, 30),
                        new Delay(2000)
                    },
                    new IAITaskParameter[]
                    {
                        new Delay(2000),
                        new SpawnEnemyInLineY(7, -0.9f),
                        new Delay(2000),
                        new SpawnEnemyInLineY(7),
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
                    new(AIRoutineType.CreateEnemyInCircle, 10000, 3, 12),
                    new(AIRoutineType.CreateEnemyRandomPos, 2400, 1, 4),
                    new(AIRoutineType.CreateMeteor, 2500, 0, 2, 0.5f),
                    new(AIRoutineType.CreateEnemyInLine, 6500, 5, 10, 0.45f)
                }
            };
        }
    }
}