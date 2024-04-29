using UnityEngine;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level06 : IAIBehavior
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
                StageLevel = 6,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(1000),
                    new CreateMeteor(1, 1000),
                    new SpawnEnemyInLineY(10)
                },
                NumberOfRandomTasksToPerform = 1,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(4, Random.Range(500,1500)),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInCircle(0.8f, Random.Range(3, 5)),
                        new Delay(300),
                        new SpawnEnemyInCircle(1.2f, Random.Range(5, 8)),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 5),
                        new Delay(4000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyInSpiral(0.6f, 1.8f, 22, 1.3f, 30),
                        new Delay(2000)
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
                    new(AIRoutineType.CreateEnemyRandomPos, 2900, 1, 4),
                    new(AIRoutineType.CreateMeteor, 4000, 0, 2, 0.5f),
                    new(AIRoutineType.CreateEnemyInLine, 7500, 3, 10, 0.45f)
                }
            };
        }
    }
}