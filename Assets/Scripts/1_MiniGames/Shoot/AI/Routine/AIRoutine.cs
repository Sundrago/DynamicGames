using System;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    /// AIRoutine refers to the continuous behavior of the game's AI, where it consistently executes predefined tasks,
    /// such as creating enemies in various shapes.
    /// </summary>
    
    public enum AIRoutineType {
        CreateEnemyInCircle,
        CreateEnemyInLine,
        CreateEnemyInSpiral,
        CreateEnemyRandomPos,
        CreateItem,
        CreateMeteor
    }
    
    public class AIRoutine
    {
        public AIRoutineType RoutineType { get; private set; }
        public AIRoutineData AIRoutineData { get; private set; }
        
        public AIRoutine(AIRoutineType routineType, int delay, int min, int max, float probability = 1f)
        {
            this.RoutineType = routineType;
            this.AIRoutineData = new AIRoutineData(delay, min, max, probability);
        }
    }
    
    public class AIRoutineData
    {
        public int Delay { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public float Probability { get; set; }

        public void Initialize(int delay, int min, int max, float probability = 1)
        {
            Delay = delay;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public AIRoutineData(int delay, int min, int max, float probability = 1)
        {
            Initialize(delay, min, max, probability);
        }
    }
}