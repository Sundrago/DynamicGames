using System;
using System.Threading.Tasks;
using DynamicGames.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    

    /// <summary>
    /// Represents a task performed by the AI at each stage.
    /// </summary>
    [Serializable]
    public class AIStageTask
    {
        public int StageLevel;
        public IAITask[] PreTasks;
        public int NumberOfRandomTasksToPerform;
        public IAITask[][] RandomTaskPool;
        public IAITask[] PostTasks;
    }

    [Serializable]
    public class StageLevel
    {
        public AIStageTask[] AIStageTasks;
    }

    public class AITaskManager : MonoBehaviour
    {
        private ItemManager itemManager;
        private GameManager gameManager;
        private EnemyManager enemyManager;

        public StageLevel AIStageTasks;
        [SerializeField] private AIBehavior AIBehaviorSO;
        
        [Button]
        private void PrintStageLevelData()
        {
            AIBehaviorSO = new AIBehavior()
            {
                StageLevel = 1,
                NumberOfRandomTasksToPerform = 1,
                PreTasks = new IAITask[]
                {
                    new SetIslandAnimation(IslandState.Open),
                    new Delay(1000),
                    new SetFaceAnimation(FaceState.TurnRed),
                    new Delay(2000)
                },
                RandomTaskPool = new IAITask[][]
                {
                    new IAITask[]
                    {
                        new CreateMeteor(1, 2001)
                    },
                    new IAITask[]
                    {
                        new SpawnEnemiesAtRandomPos(3, 100),
                        new Delay(1000)
                    },
                    new IAITask[]
                    {
                        new SpawnEnemyOnIsland(SpawnEnemyOnIslandDirection.Left, 2),
                        new SpawnEnemyOnIsland(SpawnEnemyOnIslandDirection.Right, 2)
                    },
                },
                PostTasks = new IAITask[]
                {
                    new SetIslandAnimation(IslandState.Close),
                    new Delay(1000)
                }
            };
            
            Debug.Log(JsonConvert.SerializeObject(AIBehaviorSO, new StringEnumConverter()));
        }


        private async Task ExecuteAITask(AITask task)
        {
            switch (task.AITaskType)
            {
                case AITaskType.Delay:
                    await HandleDelayTask(task);
                    break;
                case AITaskType.SpawnItem:
                    await HandleSpawnItemTask(task);
                    break;
                case AITaskType.SetIslandAnimation:
                    HandleSetIslandAnimationTask(task);
                    break;
                case AITaskType.SetFaceAnimation:
                    HandleSetFaceAnimationTask(task);
                    break;
                case AITaskType.SpawnEnemiesAtRandomPos:
                    await HandleSpawnEnemiesAtRandomPos(task);
                    break;
                case AITaskType.SpawnEnemyInSpiral:
                    await HandleSpawnEnemyInSpiralTask(task);
                    break;
                // case AITaskType.SpawnLeft:
                //     await HandleSpawnLeft(task);
                //     break;
                // case AITaskType.SpawnRight:
                //     await HandleSpawnRight(task);
                //     break;
                case AITaskType.CreateMeteor:
                    gameManager.CreateMetheor();
                    break;
            }
        }

        private async Task HandleSpawnEnemyInSpiralTask(AITask task)
        {
            try
            {
                int radiusMin = int.Parse(task.Parameters[0]);
                int radiusMax = int.Parse(task.Parameters[1]);
                int count = int.Parse(task.Parameters[2]);
                int maxAngle = int.Parse(task.Parameters[2]);
                int delay = int.Parse(task.Parameters[4]);
                int prewarmDuration = int.Parse(task.Parameters[5]);
                await enemyManager.SpawnEnemyInSpiral(radiusMin, radiusMax, count, maxAngle, delay, prewarmDuration);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private async Task HandleCreateMeteorTask(AITask task)
        {
            try
            {
                int count = int.Parse(task.Parameters[0]);
                int delay = int.Parse(task.Parameters[1]);
                for (int i = 0; i < count; i++)
                {
                    itemManager.SpawnItem();
                    await Task.Delay(delay);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private async Task HandleSpawnLeft(AITask task)
        {
            try
            {
                int count = int.Parse(task.Parameters[0]);
                FireAndForget(gameManager.SpawnOnLeft(count));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private async Task HandleSpawnRight(AITask task)
        {
            try
            {
                int count = int.Parse(task.Parameters[0]);
                FireAndForget(gameManager.SpawnOnRight(count));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private async Task HandleSpawnEnemiesAtRandomPos(AITask task)
        {
            try
            {
                int count = int.Parse(task.Parameters[0]);
                int delay = int.Parse(task.Parameters[1]);

                for (int i = 0; i < count; i++)
                {
                    enemyManager.SpawnEnemyAtRandomPos();
                    await Task.Delay(delay);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private void HandleSetIslandAnimationTask(AITask task)
        {
            if (Enum.TryParse<IslandState>(task.Parameters[0], out IslandState islandState))
            {
                gameManager.SetIslandAnimation(islandState);
            }
            else
            {
                Console.WriteLine($"Failed to parse {task.Parameters[0]} in ExecuteAITask {task.AITaskType}");
            }
        }

        private void HandleSetFaceAnimationTask(AITask task)
        {
            if (Enum.TryParse<FaceState>(task.Parameters[0], out FaceState faceState))
            {
                gameManager.SetFaceAnimation(faceState);
            }
            else
            {
                Console.WriteLine($"Failed to parse {task.Parameters[0]} in ExecuteAITask {task.AITaskType}");
            }
        }

        private async Task HandleSpawnItemTask(AITask task)
        {
            try
            {
                int count = int.Parse(task.Parameters[0]);
                int delay = int.Parse(task.Parameters[1]);

                for (int i = 0; i < count; i++)
                {
                    itemManager.SpawnItem();
                    await Task.Delay(delay);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }

        private async Task HandleDelayTask(AITask task)
        {
            try
            {
                await Task.Delay(int.Parse(task.Parameters[0]));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Exception: {e.Message}" +
                                 $"Stack Trace: {e.StackTrace}");
                ReportTaskError(task);
                throw;
            }
        }


        public static void FireAndForget(Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Debug.LogError("Task failed: " + t.Exception.Flatten());
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private void ReportTaskError(AITask task)
        {
            string errorMessage = "Error executing task: " + task.AITaskType;
            foreach (string element in task.Parameters)
            {
                errorMessage += "\n" + element;
            }

            Debug.LogError(errorMessage);
        }
    }
}