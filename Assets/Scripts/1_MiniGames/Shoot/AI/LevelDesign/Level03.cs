namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level03 : IAIBehavior
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
                StageLevel = 3,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(1000),
                    new CreateMeteor(1, 1000)
                },
                NumberOfRandomTasksToPerform = 1,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(3, 1000),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemiesAtRandomPos(5, 100),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 5),
                        new Delay(4000)
                    },
                },
                PostTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Close),
                    new Delay(1000)
                },
                PostRoutine = new AIRoutine[]
                {
                    new(AIRoutineType.CreateEnemyInCircle, 10000, 0, 4),
                    new(AIRoutineType.CreateEnemyRandomPos, 2900, 0, 4),
                    new(AIRoutineType.CreateMeteor, 4500, 0, 1, 0.5f),
                    new(AIRoutineType.CreateEnemyInLine, 7500, 3, 6, 0.3f)
                }
            };
        }
    }
}