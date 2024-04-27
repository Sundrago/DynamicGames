using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages AI behaviors and tasks in shooting mini-game.
    /// </summary>
    public class AIEnumerator : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private ItemManager itemManager;
        [SerializeField] private AIManager aiManager;
        
        public void StartTasks()
        {
            StartCoroutine(CreateEnemyAtRandomPos());
            StartCoroutine(CreateEnemyAtPlayerInCircle());
            StartCoroutine(CreateMetheors());
            StartCoroutine(CreateEnemyInLine());
            StartCoroutine(CreateEnemyInSpiral());
            StartCoroutine(CreateItem());
        }

        private IEnumerator CreateEnemyAtRandomPos()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateEnemyRandomPos;
                yield return new WaitForSeconds(info.Delay / 1000f);
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability)
                {
                    var amt = Random.Range(info.Min, info.Max + 1);
                    for (var i = 0; i < amt; i++) enemyManager.SpawnEnemyAtRandomPos();
                }
            }
        }

        private IEnumerator CreateEnemyAtPlayerInCircle()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateEnemyInCircle;
                yield return new WaitForSeconds(info.Delay / 1000);
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability)
                    enemyManager.SpawnEnemyInCircle(1f, Random.Range(info.Min, info.Max));
            }
        }

        private IEnumerator CreateMetheors()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateMeteor;
                yield return new WaitForSeconds(info.Delay / 1000);
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability)
                {
                    var amt = Random.Range(info.Min, info.Max + 1);
                    for (var i = 0; i < amt; i++)
                    {
                        yield return new WaitForSeconds(2f);
                        gameManager.CreateMetheor();
                    }
                }
            }
        }

        private IEnumerator CreateEnemyInLine()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateEnemyInLine;
                yield return new WaitForSeconds(info.Delay / 1000);
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability)
                    enemyManager.SpawnEnemyInLineY(Random.Range(info.Min, info.Max + 1));
            }
        }

        private IEnumerator CreateEnemyInSpiral()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateEnemyInSpiral;
                yield return new WaitForSeconds(info.Delay / 1000);
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability)
                    enemyManager.SpawnEnemyInSpiral(0.6f * Random.Range(0.9f, 1.1f),
                        1.5f * Random.Range(0.85f, 1.3f), Random.Range(info.Min, info.Max + 1)
                        , 1.5f * Random.Range(0.7f, 1.3f), 35, 0.6f * Random.Range(0.8f, 1.2f));
            }
        }

        private IEnumerator CreateItem()
        {
            while (gameManager.state == GameManager.ShootGameState.Playing)
            {
                var info = aiManager.CurrentAutoAttackInfo.CreateItem;
                var count = itemManager.items.Count;
                yield return new WaitForSeconds(info.Delay / 1000);
                info.Probability = (1 - 0.4f * count) * 0.85f;
                if (info.Max != 0 && Random.Range(0f, 1f) < info.Probability) itemManager.SpawnItem();
            }
        }
    }
}