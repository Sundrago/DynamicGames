namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public class Level01 : IAIBehavior
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
                StageLevel = 1,
                PreRoutine = new AIRoutine[]
                {
                    new(AIRoutineType.CreateEnemyInCircle, 1000, 0, 0),
                    new(AIRoutineType.CreateEnemyRandomPos, 1100, 0, 0),
                    new(AIRoutineType.CreateMeteor, 1200, 0, 0),
                    new(AIRoutineType.CreateEnemyInLine, 1300, 0, 0),
                    new(AIRoutineType.CreateEnemyInSpiral, 1400, 0, 0),
                    new(AIRoutineType.CreateItem, 1500, 1, 1, 0.25f)
                },
                PreTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.TurnRed),
                    new Delay(2000)
                },
                NumberOfRandomTasksToPerform = 1,
                RandomTaskPool = new IAITaskParameter[][]
                {
                    new IAITaskParameter[]
                    {
                        new CreateMeteor(1, 2000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemiesAtRandomPos(3, 100),
                        new Delay(1000)
                    },
                    new IAITaskParameter[]
                    {
                        new SpawnEnemyOnIsland(SpawnDirection.Both, 2),
                    },
                },
                PostTasks = new IAITaskParameter[]
                {
                    new SetIslandAnimation(IslandState.Close),
                    new Delay(1000)
                },
                PostRoutine = new AIRoutine[]
                {
                    new(AIRoutineType.CreateEnemyInCircle, 12000, 0, 1),
                    new(AIRoutineType.CreateEnemyRandomPos, 3000, 0, 2),
                    new(AIRoutineType.CreateMeteor, 1000, 0, 0)
                }
            };
        }
    }
}