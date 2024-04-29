using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    public enum SpawnDirection
    {
        Left,
        Right,
        Both
    }
    public class SpawnEnemyOnIsland : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemyOnIsland;
        public SpawnDirection Direction { get; }
        public int Amount { get; }

        [JsonConstructor]
        public SpawnEnemyOnIsland(SpawnDirection direction, int amount)
        {
            Direction = direction;
            Amount = amount;
        }
    }
    
    public partial class AIManager
    {
        private async Task SpawnEnemyOnIsland(IAITaskParameter taskParameter)
        {
            var spawnEnemyOnIslandTask = taskParameter as SpawnEnemyOnIsland;
            if (spawnEnemyOnIslandTask.Direction == SpawnDirection.Left)
                await gameManager.AIDoorController.SpawnOnLeft(spawnEnemyOnIslandTask.Amount);
            else if (spawnEnemyOnIslandTask.Direction == SpawnDirection.Right)
                await gameManager.AIDoorController.SpawnOnRight(spawnEnemyOnIslandTask.Amount);
            else
            {
                Task.Run(()=> gameManager.AIDoorController.SpawnOnLeft(spawnEnemyOnIslandTask.Amount));
                Task.Run(()=> gameManager.AIDoorController.SpawnOnRight(spawnEnemyOnIslandTask.Amount));
                // await gameManager.SpawnOnRight(spawnEnemyOnIslandTask.Amount);
            }
        }
    }
}