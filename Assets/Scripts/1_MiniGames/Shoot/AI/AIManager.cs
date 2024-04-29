using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicGames.MiniGames.Shoot.LevelDesign;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DynamicGames.MiniGames.Shoot
{ 
    public class AIRoutineHolder
    {
        public AIRoutineData CreateEnemyInCircle;
        public AIRoutineData CreateEnemyInLine;
        public AIRoutineData CreateEnemyInSpiral; 
        public AIRoutineData CreateEnemyRandomPos; 
        public AIRoutineData CreateItem;
        public AIRoutineData CreateMeteor;
    }
    
    public enum AITaskType
    {
        Delay,
        SpawnItem,
        SetIslandAnimation,
        SetFaceAnimation,
        SpawnEnemiesAtRandomPos,
        SpawnEnemyInSpiral,
        SpawnEnemyOnIsland,
        SpawnEnemyInCircle,
        SpawnEnemyInLineY,
        CreateMeteor
    }
    
    public partial class AIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private ItemManager itemManager;
        [SerializeField] private TextAsset defaultAutoAttackJson;

        [SerializeReference] private IAIBehavior[] behaviors;

        public AIRoutineHolder CurrentRoutine;
        public enum AIState
        {
            Waiting, Running
        }
        
        private AIState state = AIState.Waiting;
        public bool cancelRequest = false;

        public void SetDefaultState()
        {
            CurrentRoutine = JsonConvert.DeserializeObject<AIRoutineHolder>(defaultAutoAttackJson.text);
        }

        public void StartStage(int idx)
        {
            if (idx >= behaviors.Length)
            {
                Debug.LogError("StartStage Index out of range : " + idx);
                return;
            }
            cancelRequest = false;
            var task = HandleBehavior(behaviors[idx].AIBehavior);
        }

        private async Task HandleBehavior(AIBehavior aiBehavior)
        {
            state = AIState.Running;
            Debug.Log($"Starting AI Stage : {aiBehavior.StageLevel}");
            
            UpdateAIRoutine(aiBehavior.PreRoutine);
            await HandleTasks(aiBehavior.PreTasks);
            for (int i = 0; i < aiBehavior.NumberOfRandomTasksToPerform; i++)
            {
                if(cancelRequest) break;
                int randomIdx = Random.Range(0, aiBehavior.RandomTaskPool.Length);
                await HandleTasks(aiBehavior.RandomTaskPool[randomIdx]);
            }
            await HandleTasks(aiBehavior.PostTasks);
            UpdateAIRoutine((aiBehavior.PostRoutine));
            
            state = AIState.Waiting;
        }

        public bool IsRunningTasks => state == AIState.Running;
        
        private async Task HandleTasks(IAITaskParameter[] tasks)
        {
            foreach (var task in tasks)
            {
                if(cancelRequest) break;
                Debug.Log($"Performing AI Task : {task.AITaskType}");
                switch (task.AITaskType)
                {
                    case AITaskType.Delay:
                        await Delay(task);
                        break;
                    case AITaskType.SpawnItem:
                        await SpawnItem(task);
                        break;
                    case AITaskType.SetIslandAnimation:
                        await SetIslandAnimation(task);
                        break;
                    case AITaskType.SetFaceAnimation:
                        await SetFaceAnimation(task);
                        break;
                    case AITaskType.SpawnEnemiesAtRandomPos:
                        await SpawnEnemiesAtRandomPos(task);
                        break;
                    case AITaskType.SpawnEnemyInSpiral:
                        await DoEnemySpiral(task);
                        break;
                    case AITaskType.SpawnEnemyOnIsland:
                        await SpawnEnemyOnIsland(task);
                        break;
                    case AITaskType.CreateMeteor:
                        await CreateMeteor(task);
                        break;
                    case AITaskType.SpawnEnemyInLineY:
                        await SpawnEnemyInLineY(task);
                        break;
                    case AITaskType.SpawnEnemyInCircle:
                        await SpawnEnemyInCircle(task);
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateAIRoutine(AIRoutine[] aiRoutines)
        {
            foreach (var routine in aiRoutines)
            {
                Debug.Log(routine.RoutineType);
                switch (routine.RoutineType)
                {
                    case AIRoutineType.CreateEnemyInCircle:
                        CurrentRoutine.CreateEnemyInCircle = routine.AIRoutineData;
                        break;
                    case AIRoutineType.CreateEnemyInLine:
                        CurrentRoutine.CreateEnemyInLine = routine.AIRoutineData;
                        break;
                    case AIRoutineType.CreateEnemyInSpiral:
                        CurrentRoutine.CreateEnemyInSpiral = routine.AIRoutineData;
                        break;
                    case AIRoutineType.CreateEnemyRandomPos:
                        CurrentRoutine.CreateEnemyRandomPos = routine.AIRoutineData;
                        break;
                    case AIRoutineType.CreateItem:
                        CurrentRoutine.CreateItem = routine.AIRoutineData;
                        break;
                    case AIRoutineType.CreateMeteor:
                        CurrentRoutine.CreateMeteor = routine.AIRoutineData;
                        break;
                }
            }
        }
    }
}