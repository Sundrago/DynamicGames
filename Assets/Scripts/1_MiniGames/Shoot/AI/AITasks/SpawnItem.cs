using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class SpawnItem : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SpawnItem;
        public int Amount { get; }
        public int Delay { get; }

        [JsonConstructor]
        public SpawnItem(int amount, int delay)
        {
            Amount = amount;
            Delay = delay;
        }
    }

    public partial class AIManager
    {
        private async Task SpawnItem(IAITaskParameter taskParameter)
        {
            var spawnItemTask = taskParameter as SpawnItem;
            if (spawnItemTask == null)
            {
                throw new InvalidCastException("Failed to convert IAITaskParameter to SpawnItem");
            }

            for (int i = 0; i < spawnItemTask.Amount; i++)
            {
                itemManager.SpawnItem();
                await Task.Delay(spawnItemTask.Delay);
            }
        }
    }
}