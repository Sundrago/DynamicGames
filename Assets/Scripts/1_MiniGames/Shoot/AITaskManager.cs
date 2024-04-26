using System;
using System.Threading.Tasks;
using DynamicGames.UI;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    [Serializable]
    public enum AITaskType
    {
        Delay,
        SpawnItem,
        SetIslandAnimation,
        SetFaceAnimation,
        SpawnEnemiesAtRandomPos,
        SpawnEnemyInSpiral,
        SpawnLeft,
        SpawnRight,
        CreateMeteor
    }

    /// <summary>
    /// Represents a single task that could be performed by the AI.
    /// </summary>
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

    /// <summary>
    /// Represents a task performed by the AI at each stage.
    /// </summary>
    [Serializable]
    public class AIStageTask
    {
        public int StageLevel;
        public AITask[] PreTasks;
        public int NumberOfRandomTasksToPerform;

        /// The pool of tasks from which the AI randomly selects tasks to perform.
        public AITask[][] RandomTaskPool;
        public AITask[] PostTasks;
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

        [Button]
        private void PrintStageLevelData()
        {
            AIStageTasks = new StageLevel()
            {
                AIStageTasks = new []
                {
                    new AIStageTask
                {
                    StageLevel = 1,
                    PreTasks = new AITask[]
                    {
                        new AITask(AITaskType.SetIslandAnimation, new[] { "Open" }),
                        new AITask(AITaskType.Delay, new[] { "1000" }),
                        new AITask(AITaskType.SetFaceAnimation, new[] { "TurnRed" }),
                        new AITask(AITaskType.Delay, new[] { "2000" }),
                    },
                    NumberOfRandomTasksToPerform = 1,
                    RandomTaskPool = new AITask[][]
                    {
                        new AITask[]
                        {
                            new AITask(AITaskType.CreateMeteor, new[] { "1", "2000" }),
                        },
                        new AITask[]
                        {
                            new AITask(AITaskType.SpawnEnemiesAtRandomPos, new[] { "3", "100" }),
                            new AITask(AITaskType.Delay, new[] { "1000" }),
                        },
                        new AITask[]
                        {
                            new AITask(AITaskType.SpawnLeft, new[] { "2" }),
                            new AITask(AITaskType.SpawnRight, new[] { "2" }),
                        },
                    },
                    PostTasks = new AITask[]
                    {
                        new AITask(AITaskType.SetIslandAnimation, new[] { "Close" }),
                        new AITask(AITaskType.Delay, new[] { "1000" }),
                    }
                },

                new AIStageTask
                {
                    StageLevel = 2,
                    PreTasks = new AITask[] { },
                    NumberOfRandomTasksToPerform = 1,
                    RandomTaskPool = new AITask[][]
                    {
                        new AITask[] { },
                        new AITask[] { },
                        new AITask[] { },
                    },
                    PostTasks = new AITask[] { }
                },
                }
            };
            
            Debug.Log(JsonUtility.ToJson(AIStageTasks, true));
            Debug.Log(JsonConvert.SerializeObject(AIStageTasks));
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
                case AITaskType.SpawnLeft:
                    await HandleSpawnLeft(task);
                    break;
                case AITaskType.SpawnRight:
                    await HandleSpawnRight(task);
                    break;
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
            if (Enum.TryParse<GameManager.IslandState>(task.Parameters[0], out GameManager.IslandState islandState))
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
            if (Enum.TryParse<GameManager.FaceState>(task.Parameters[0], out GameManager.FaceState faceState))
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