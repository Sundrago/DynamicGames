using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class Delay : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.Delay;
        public int Duration { get; private set; }

        [JsonConstructor]
        public Delay(int duration)
        {
            Duration = duration;
        }
    }

    public partial class AIManager
    {
        private static async Task Delay(IAITaskParameter taskParameter)
        {
            var delayTask = taskParameter as Delay;
            if (delayTask == null)
            {
                throw new InvalidCastException("Failed to convert IAITaskParameter to Delay");
            }

            await Task.Delay(delayTask.Duration);
        }
    }
}