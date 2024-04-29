using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    public class SpawnEnemyInSpiral : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyInSpiral;

        public float RadiusMin { get; private set; }
        public float RadiusMax { get; private set; }
        public int Count { get; private set; }
        public float MaxAngle { get; private set; }
        public int Delay { get; private set; }
        public float PrewarmDuration { get; private set; }
        
        [JsonConstructor]
        public SpawnEnemyInSpiral(float radiusMin, float radiusMax, int count, float maxAngle, int delay,
            float prewarmDuration = 0.75f)
        {
            RadiusMin = radiusMin;
            RadiusMax = radiusMax;
            Count = count;
            MaxAngle = maxAngle;
            Delay = delay;
            PrewarmDuration = prewarmDuration;
        }
    }
    
    public partial class AIManager
    {
        private async Task DoEnemySpiral(IAITaskParameter taskParameter)
        {
            var spawnEnemyInSpiralTask = taskParameter as SpawnEnemyInSpiral;
            await enemyManager.SpawnEnemyInSpiral(spawnEnemyInSpiralTask.RadiusMin,
                spawnEnemyInSpiralTask.MaxAngle, spawnEnemyInSpiralTask.Count,
                spawnEnemyInSpiralTask.MaxAngle, spawnEnemyInSpiralTask.Delay);
        }
    }
}