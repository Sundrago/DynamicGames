// using System.Collections.Generic;
// using System.IO;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace DynamicGames.MiniGames.Shoot.LevelDesign
// {
//     public class JsonWriter : MonoBehaviour
//     {
//         [SerializeReference] private IAIBehavior[] behaviors;
//         [SerializeField] private List<AIBehavior> AIBehaviors;
//         
//         [Button]
//         private void WriteJson()
//         {
//             AIBehaviors = new List<AIBehavior>();
//             for (int i = 0; i < behaviors.Length; i++)
//             {
//                 var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
//                 string json = Newtonsoft.Json.JsonConvert.SerializeObject(behaviors[i].AIBehavior, settings);
//                 Debug.Log(json);
//                 File.WriteAllText(Application.dataPath + "/Resources/Level_"+(i+1).ToString("D2")+".json", json);
//                 AIBehaviors.Add(JsonConvert.DeserializeObject<AIBehavior>(json, settings));
//             }
//         }
//
//         // [Button]
//         // private void ReadJson()
//         // {
//         //     AIBehavior[] aiBehaviors = new AIBehavior[AIBehaviors.Count];
//         //     for (int i = 0; i < AIBehaviors.Count; i++)
//         //     {
//         //         AIBehavior behavior = Newtonsoft.Json.JsonConvert.DeserializeObject<AIBehavior>(AIBehaviors[i].text);
//         //         aiBehaviors[i] = behavior;
//         //     }
//         // }
//     }
// }