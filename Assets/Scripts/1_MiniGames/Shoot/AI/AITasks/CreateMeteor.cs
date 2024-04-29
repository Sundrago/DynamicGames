using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    /// Represents a task parameter for creating meteors in the game.
    /// </summary>
    public class CreateMeteor : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.CreateMeteor;
        public int Amount { get; }
        public int Delay { get; }
        
        [JsonConstructor]
        public CreateMeteor(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }

    /// <summary>
    /// Creates meteors in the shoot mini-game.
    /// </summary>
    public partial class AIManager
    {
        private async Task CreateMeteor(IAITaskParameter taskParameter)
        {
            var createMeteor = taskParameter as CreateMeteor;
            for (int i = 0; i < createMeteor.Amount; i++)
            {
                gameManager.ItemHandler.CreateMetheor();
                await Task.Delay(createMeteor.Delay);
            }
        }
    }
}