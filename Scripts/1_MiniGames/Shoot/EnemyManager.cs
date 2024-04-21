using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Class responsible for managing enemies in a shooting mini-game.
    /// </summary>
    public class EnemyManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private EnemyController enemyController;
        [SerializeField] private Transform player, island;
        [SerializeField] private int defaultCapacity, maxCapacity;
        
        private ObjectPool<EnemyController> enemyObjectPool;
        private Vector2 screenBounds;
        private bool SpawingOnSpiral;

        public List<EnemyController> enemyControllers { get; private set; }

        public static EnemyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            enemyControllers = new List<EnemyController>();
            enemyObjectPool = GetEnemyObjectPool();
            screenBounds = GetScreenBound();
        }

        private ObjectPool<EnemyController> GetEnemyObjectPool()
        {
            return new ObjectPool<EnemyController>(() => { return Instantiate(enemyController); },
                obj => { obj.gameObject.SetActive(true); }, obj => { obj.gameObject.SetActive(false); },
                obj => { Destroy(obj); }, true, defaultCapacity, maxCapacity);
        }

        private Vector2 GetScreenBound()
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                Camera.main.transform.position.z));
        }

        public void SpawnEnemy(Vector2 pos, bool forceCreate = false, float delay = 0)
        {
            if (GameManager.Instacne.state != GameManager.ShootGameState.Playing) return;
            if (!forceCreate && SpawingOnSpiral) return;

            var enemyController = enemyObjectPool.Get();
            enemyController.transform.SetParent(gameObject.transform);
            enemyController.transform.position = pos;
            enemyController.Init(player, 0.4f, delay);
            enemyControllers.Add(enemyController);
        }

        private Vector2 GetRandomPosOnScreen()
        {
            return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
        }

        public void DestroyEnemy(EnemyController enemyController)
        {
            enemyControllers.Remove(enemyController);
            enemyObjectPool.Release(enemyController);
        }

        public void SpawnEnemyInCircle(float radius, int count)
        {
            Vector2 playerPos = player.transform.position;

            for (var i = 0; i < count; i++)
            {
                var angle = 1f / count * 2f * Mathf.PI * i;
                var pos = playerPos + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
                SpawnEnemy(pos);
            }
        }

        public async Task SpawnEnemyInSpiral(float radiusMin, float radiusMax, int count, float maxAngle = 1.5f,
            int delay = 150, float prewarmDuration = 0.5f)
        {
            SpawingOnSpiral = true;

            Vector2 playerPos = player.transform.position;
            var randomAngle = Random.Range(0f, 2f);

            for (var i = 0; i < count; i++)
            {
                var position = GetPosInSpiral(i);
                SpawnEnemy(position, true, prewarmDuration);
                await Task.Delay(delay);
            }

            SpawingOnSpiral = false;

            Vector2 GetPosInSpiral(int i)
            {
                var normal = 1f / count * i;
                var angle = (randomAngle + normal * maxAngle) * 2f * Mathf.PI;
                var radius = radiusMin + (radiusMax - radiusMin) * normal;
                return playerPos + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            }
        }

        public void SpawnEnemyInLineY(int count, float normalY = -0.9f)
        {
            for (var i = 0; i < count; i++)
            {
                var normalX = 2f / (count + 1) * (i + 1) - 1f;
                var pos = new Vector2(normalX * screenBounds.x, normalY * screenBounds.y);
                SpawnEnemy(pos);
            }
        }

        public void SpawnEnemyAtRandomPos()
        {
            SpawnEnemy(GetRandomPosOnScreen());
        }

        public void SpawnOnIsland(int angle, float x, float y)
        {
            if (GameManager.Instacne.state != GameManager.ShootGameState.Playing) return;
            var enemyController = enemyObjectPool.Get();
            enemyController.transform.SetParent(gameObject.transform);
            enemyController.transform.position = new Vector3(island.position.x, island.position.y, 0f);
            enemyController.Init(player, 0.4f, 1f, angle, x, y);

            enemyControllers.Add(enemyController);
        }

        public void GameOver()
        {
            for (var i = 0; i < enemyControllers.Count; i++) enemyControllers[i].enemyStats = EnemyStats.GameOver;

            SpawingOnSpiral = false;
        }

        public void KillAll()
        {
            for (var i = enemyControllers.Count - 1; i >= 0; i--)
            {
                if (enemyControllers[i].enemyStats == EnemyStats.Despawning) continue;
                DestroyEnemy(enemyControllers[i]);
            }

            enemyControllers.Clear();
            SpawingOnSpiral = false;
        }

        public void Revive()
        {
            for (var i = 0; i < enemyControllers.Count; i++) enemyControllers[i].enemyStats = EnemyStats.Ready;
            SpawingOnSpiral = false;
        }
    }
}