using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.Shoot
{
    /// <summary>
    ///     Class for managing AI in the Shoot game.
    /// </summary>
    public class AIManager : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private EnemyManager enemyManager;
        [SerializeField] private ItemManager itemManager;

        // private List<Task> tasks;
        // private bool initiated;
        //
        // private void Start()
        // {
        //     if (initiated) return;
        //     tasks = new List<Task>
        //     {
        //         CreateTask(CreateEnemyAtRandomPos),
        //         CreateTask(CreateEnemyAtPlayerInCircle),
        //         CreateTask(CreateMetheors),
        //         CreateTask(CreateEnemyInLine),
        //         CreateTask(CreateEnemyInSpiral),
        //         CreateTask(CreateItem)
        //     };
        //     initiated = true;
        // }
        // private Task CreateTask(Func<Task> taskFunction)
        // {
        //     return Task.Run(taskFunction);
        // }
        public void StartTasks()
        {
            // if (!initiated) Start();
            // await Task.WhenAll(tasks).ConfigureAwait(false);
            // CreateEnemyAtRandomPos();
            // CreateEnemyAtPlayerInCircle();
            // CreateMetheors();
            // CreateEnemyInLine();
            // CreateEnemyInSpiral();
            // CreateItem();

            StartCoroutine(CreateEnemyAtRandomPos());
            StartCoroutine(CreateEnemyAtPlayerInCircle());
            StartCoroutine(CreateMetheors());
            StartCoroutine(CreateEnemyInLine());
            StartCoroutine(CreateEnemyInSpiral());
            StartCoroutine(CreateItem());
            

        }
        private IEnumerator CreateEnemyAtRandomPos()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createEnemyRandomPos;
                yield return new WaitForSeconds(info.delay/1000f);
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
                {
                    var amt = Random.Range(info.min, info.max + 1);
                    for (var i = 0; i < amt; i++) enemyManager.SpawnEnemyAtRandomPos();
                }
            }
        }
        private IEnumerator CreateEnemyAtPlayerInCircle()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createEnemyInCircle;
                yield return new WaitForSeconds(info.delay / 1000);
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
                    enemyManager.SpawnEnemyInCircle(1f, Random.Range(info.min, info.max));
            }
        }
        private IEnumerator CreateMetheors()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createMetheor;
                yield return new WaitForSeconds(info.delay / 1000);
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
                {
                    var amt = Random.Range(info.min, info.max + 1);
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
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createEnemyInLine;
                yield return new WaitForSeconds(info.delay / 1000);
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
                    enemyManager.SpawnEnemyInLineY(Random.Range(info.min, info.max + 1));
            }
        }
        private IEnumerator CreateEnemyInSpiral()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createEnemyInSpira;
                yield return new WaitForSeconds(info.delay / 1000);
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability)
                    enemyManager.SpawnEnemyInSpiral(0.6f * Random.Range(0.9f, 1.1f),
                        1.5f * Random.Range(0.85f, 1.3f), Random.Range(info.min, info.max + 1)
                        , 1.5f * Random.Range(0.7f, 1.3f), 35, 0.6f * Random.Range(0.8f, 1.2f));
            }
        }
        private IEnumerator CreateItem()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                var info = gameManager.createItem;
                var count = itemManager.items.Count;
                yield return new WaitForSeconds(info.delay / 1000);
                info.probability = (1 - 0.4f * count) * 0.85f;
                if (info.max != 0 && Random.Range(0f, 1f) < info.probability) itemManager.SpawnItem();
            }
        }
    }
}