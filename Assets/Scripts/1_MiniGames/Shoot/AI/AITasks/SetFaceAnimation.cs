using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public enum FaceState
    {
        Idle,
        TurnRed,
        Angry01,
    }
    public class SetFaceAnimation : IAITaskParameter
    {
        public AITaskType AITaskType => AITaskType.SetFaceAnimation;
        public FaceState FaceState { get; }

        [JsonConstructor]
        public SetFaceAnimation(FaceState faceState)
        {
            FaceState = faceState;
        }
    }

    public partial class AIManager
    {
        private async Task SetFaceAnimation(IAITaskParameter taskParameter)
        {
            var setFaceAnimationTask = taskParameter as SetFaceAnimation;
            gameManager.UIManager1.SetFaceAnimation(setFaceAnimationTask.FaceState, gameManager);
            await Task.Delay(500);
        }
    }
}