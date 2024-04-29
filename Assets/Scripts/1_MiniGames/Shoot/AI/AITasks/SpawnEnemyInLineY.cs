using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    public class SpawnEnemyInLineY : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyInLineY;

        public int Count { get; }
        public float NormalYPos { get;  }
        
        [JsonConstructor]
        public SpawnEnemyInLineY(int count, float normalYPos = -0.9f)
        {
            NormalYPos = normalYPos;
        }
    }
    
    public partial class AIManager
    {
        private async Task SpawnEnemyInLineY(IAITaskParameter taskParameter)
        {
            var spawnEnemyInLineYTask = taskParameter as SpawnEnemyInLineY;
            enemyManager.SpawnEnemyInLineY(spawnEnemyInLineYTask.Count, spawnEnemyInLineYTask.NormalYPos);
            await Task.Delay(500);
        }
    }
}