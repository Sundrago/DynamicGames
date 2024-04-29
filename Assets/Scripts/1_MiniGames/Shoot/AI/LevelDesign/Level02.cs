namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level02 : IAIBehavior
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
                StageLevel = 2,
                PreRoutine = new AIRoutine[]
                {
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.Angry01),
                    new Delay(2000),
                    new CreateMeteor(1, 1000)
                },
                NumberOfRandomTasksToPerform = 1,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(2, 1000),
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemiesAtRandomPos(4, 100),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 4),
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
                    new(AIRoutineType.CreateEnemyInCircle, 10000, 0, 1),
                    new(AIRoutineType.CreateEnemyRandomPos, 2900, 0, 2),
                    new(AIRoutineType.CreateMeteor, 1000, 0, 0)
                }
            };
        }
    }
}