using System;

namespace DynamicGames.MiniGames.Shoot.LevelDesign
{
    public interface IAIBehavior
    {
        public AIBehavior AIBehavior { get; }
    }

    [Serializable]
    public class AIBehavior
    {
        public int StageLevel; 
        public AIRoutine[] PreRoutine; 
        public IAITaskParameter[] PreTasks;
        
        public int NumberOfRandomTasksToPerform; 
        public IAITaskParameter[][] RandomTaskPool;
        
        public IAITaskParameter[] PostTasks; 
        public AIRoutine[] PostRoutine;
    }
}