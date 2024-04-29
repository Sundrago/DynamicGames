using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    /// AITask is a behavior that triggers when specific conditions in the game are met, such as the player reaching a certain score or achieving a particular milestone.
    /// </summary>
    public interface IAITaskParameter
    {
        public abstract AITaskType AITaskType { get; }
    }
}