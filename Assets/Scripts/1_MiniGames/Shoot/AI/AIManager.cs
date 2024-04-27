using System.Collections.Generic;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public class AutoAttackInfo
    {
        public int Delay;
        public int Min;
        public int Max;
        public float Probability;

        public void Initialize(int delay, int min, int max, float probability = 1)
        {
            Delay = delay;
            Min = min;
            Max = max;
            Probability = probability;
        }

        public AutoAttackInfo(int delay, int min, int max, float probability = 1)
        {
            Initialize(delay, min, max, probability);
        }
    }

    public class AutoAttackInfoHolder
    {
        public AutoAttackInfo CreateEnemyInCircle;
        public AutoAttackInfo CreateEnemyInLine;
        public AutoAttackInfo CreateEnemyInSpiral; 
        public AutoAttackInfo CreateEnemyRandomPos; 
        public AutoAttackInfo CreateItem;
        public AutoAttackInfo CreateMeteor;
    }
    
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [SerializeField] private TextAsset defaultAutoAttackJson;
        [TextArea] public string defaultAutoAttack;

        public AutoAttackInfoHolder CurrentAutoAttackInfo;
        
        [Button]
        public void SetDefaultState()
        {
            CurrentAutoAttackInfo = DeSerializeAutoAttackData(defaultAutoAttackJson.text);
        }

        private AutoAttackInfoHolder DeSerializeAutoAttackData(string input)
        {
            return JsonConvert.DeserializeObject<AutoAttackInfoHolder>(input);
        }

        private void SerializeAutoAttackDaeta(AutoAttackInfoHolder attackInfoHolder)
        {
            defaultAutoAttack = JsonConvert.SerializeObject(attackInfoHolder);
        }
    }
}