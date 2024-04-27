using System;
using DynamicGames.MiniGames.Shoot;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    [CreateAssetMenu(fileName = "AIBehavior", menuName = "Shoot/New Behavior")]
    public class AIBehavior : ScriptableObject
    {
        [SerializeField] public int StageLevel;
        [SerializeReference] public IAITask[] PreTasks;
        [SerializeField] public int NumberOfRandomTasksToPerform;
        [SerializeReference] public IAITask[][] RandomTaskPool;
        [SerializeReference] public IAITask[] PostTasks;

        // [Button]
        // private void SerializeTest()
        // {
        //     StageLevel = 1;
        //     PreTasks = new IAITask[]
        //     {
        //         new SetIslandAnimation(IslandState.Open),
        //         new Delay(1000),
        //         new SetFaceAnimation(FaceState.TurnRed),
        //         new Delay(2000)
        //     };
        //     NumberOfRandomTasksToPerform = 1;
        //     RandomTaskPool = new IAITask[][]
        //     {
        //         new IAITask[]
        //         {
        //             new CreateMeteor(1, 2000)
        //         },
        //         new IAITask[]
        //         {
        //             new SpawnEnemiesAtRandomPos(3, 100),
        //             new Delay(1000)
        //         },
        //         new IAITask[]
        //         {
        //             new SpawnEnemyOnIsland(SpawnEnemyOnIslandDirection.Left, 2),
        //             new SpawnEnemyOnIsland(SpawnEnemyOnIslandDirection.Right, 2)
        //         },
        //     };
        //     PostTasks = new IAITask[]
        //     {
        //         new SetIslandAnimation(IslandState.Close),
        //         new Delay(1000)
        //     };
        // }
    }
}

[Serializable]
public class AITask
{
    public AITaskType AITaskType;
    public string[] Parameters;

    public AITask(AITaskType aiTaskType, string[] parameters)
    {
        AITaskType = aiTaskType;
        Parameters = parameters;
    }
}