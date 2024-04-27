// using System;
//
// namespace DynamicGames.MiniGames.Shoot
// {
//     using UnityEditor;
//     using UnityEngine;
//
//     [CustomEditor(typeof(AIBehavior))]
//     public class AIBehaviorEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI(); // Draw the default inspector
//
//             AIBehavior stageTask = (AIBehavior)target;
//
//             // If needed, add controls to manipulate PreTasks, RandomTaskPool, and PostTasks
//             if (GUILayout.Button("Add New PreTask"))
//             {
//                 // Logic to add a new PreTask
//             }
//
//             // Handle displaying and editing of tasks
//             DisplayTasks(stageTask.preTasks);
//         }
//
//         private void DisplayTasks(IAITask[] tasks)
//         {
//             if (tasks == null) return;
//
//             foreach (IAITask task in tasks)
//             {
//                 EditorGUILayout.BeginHorizontal();
//                 EditorGUILayout.LabelField(task.GetType().Name);
//                 if (GUILayout.Button("Edit"))
//                 {
//                     // Logic to edit this task
//                 }
//
//                 if (GUILayout.Button("Remove"))
//                 {
//                     // Logic to remove this task
//                 }
//
//                 EditorGUILayout.EndHorizontal();
//             }
//         }
//         
//         private void AddTask(AIStageTask stageTask)
//         {
//             // Example of adding a specific type of task
//             // You might want to create a selection menu to choose the type of task to add
//             stageTask.PreTasks = AddTaskToArray(stageTask.PreTasks, new Delay(5));
//         }
//
//         private IAITask[] AddTaskToArray(IAITask[] tasks, IAITask newTask)
//         {
//             int newSize = tasks == null ? 1 : tasks.Length + 1;
//             IAITask[] newTasks = new IAITask[newSize];
//             if (tasks != null)
//                 tasks.CopyTo(newTasks, 0);
//             newTasks[newSize - 1] = newTask;
//             return newTasks;
//         }
//         
//         private void AddTask(AIBehavior stageTask)
//         {
//             GenericMenu menu = new GenericMenu();
//             foreach (AITaskType taskType in Enum.GetValues(typeof(AITaskType)))
//             {
//                 menu.AddItem(new GUIContent(taskType.ToString()), false, () => OnAddTaskSelected(taskType, stageTask));
//             }
//             menu.ShowAsContext();
//         }
//
//         private void OnAddTaskSelected(AITaskType taskType, AIBehavior stageTask)
//         {
//             // Create an instance of the selected task type
//             IAITask newTask = CreateTaskByType(taskType);
//             stageTask.preTasks = AddTaskToArray(stageTask.preTasks, newTask);
//         }
//
//         private IAITask CreateTaskByType(AITaskType taskType)
//         {
//             switch (taskType)
//             {
//                 case AITaskType.Delay:
//                     return new Delay(5);
//                 case AITaskType.SpawnItem:
//                     return new SpawnItem(1, 10);
//                 // Add other cases as necessary
//             }
//             return null;
//         }
//
//     }
// }