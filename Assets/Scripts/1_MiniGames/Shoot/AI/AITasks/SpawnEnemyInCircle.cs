using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    public class SpawnEnemyInCircle : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyInCircle;
        
        public float Radius { get; }
        public int Count { get; }

        [JsonConstructor]
        public SpawnEnemyInCircle(float radius, int count)
        {
            Radius = radius;
            Count = count;
        }
    }

    public partial class AIManager
    {
        private async Task SpawnEnemyInCircle(IAITaskParameter taskParameter)
        {
            var spawnEnemyInCircleTask = taskParameter as SpawnEnemyInCircle;
            enemyManager.SpawnEnemyInCircle(spawnEnemyInCircleTask.Radius, spawnEnemyInCircleTask.Count);
            await Task.Delay(500);
        }
    }
}