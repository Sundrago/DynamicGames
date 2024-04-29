using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public enum IslandState
    {
        Open,
        Close
    }
    
    public class SetIslandAnimation : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SetIslandAnimation; 
        public IslandState IslandState { get; }

        [JsonConstructor]
        public SetIslandAnimation(IslandState islandState)
        {
            IslandState = islandState;
        }
    }
    
    public partial class AIManager
    {
        private async Task SetIslandAnimation(IAITaskParameter taskParameter)
        {
            var setIslandAnimationTask = taskParameter as SetIslandAnimation;
            gameManager.AIDoorController1.SetIslandAnimation(setIslandAnimationTask.IslandState, gameManager);
            await Task.Delay(500);
        }
    }
}