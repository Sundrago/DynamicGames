using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    public class SpawnEnemiesAtRandomPos : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnEnemiesAtRandomPos;

        public int Amount { get; }
        public int Delay { get; }

        [JsonConstructor]
        public SpawnEnemiesAtRandomPos(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }
    
    public partial class AIManager
    {
        private async Task SpawnEnemiesAtRandomPos(IAITaskParameter taskParameter)
        {
            var spawnEnemiesAtRandomPosTask = taskParameter as SpawnEnemiesAtRandomPos;
            for (int i = 0; i < spawnEnemiesAtRandomPosTask.Amount; i++)
            {
                itemManager.SpawnItem();
                await Task.Delay(spawnEnemiesAtRandomPosTask.Delay);
            }
        }
        
    }
}