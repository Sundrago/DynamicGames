using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

public class Shoot_Enemy_Manager : MonoBehaviour
{
    [SerializeField] Shoot_enemy _enemy;
    [SerializeField] Transform player;
    [SerializeField] int defaultCapacity, maxCapacity;
    [SerializeField] Transform island;

    private ObjectPool<Shoot_enemy> enemy_pool;
    private Vector2 screenBounds;

    public static Shoot_Enemy_Manager Instance;

    public List<Shoot_enemy> enemy_list = new List<Shoot_enemy>();
    private bool SpawingOnSpiral = false;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enemy_pool = new ObjectPool<Shoot_enemy>(() =>
        {
            return Instantiate(_enemy);
        }, obj =>
        {
            obj.gameObject.SetActive(true);
        }, obj =>
        {
            obj.gameObject.SetActive(false);
        }, obj =>
        {
            Destroy(obj);
        }, true, defaultCapacity, maxCapacity);

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

    }

    public void SpawnEnemy(Vector2 pos, bool forceCreate = false,  float delay = 0)
    {
        if (Shoot_GameManager.Instacne.state != Shoot_GameManager.ShootGameState.playing) return;
        if(!forceCreate && SpawingOnSpiral) return;
        
        Shoot_enemy enemy = enemy_pool.Get();
        enemy.transform.SetParent(gameObject.transform);
        enemy.transform.position = pos;
        enemy.Init(player, 0.4f, delay);

        enemy_list.Add(enemy);
    }

    private Vector2 GetRandomPosOnScreen()
    {
        return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
    }

    public void DestroyEnemy(Shoot_enemy enemy)
    {
        enemy_list.Remove(enemy);
        enemy_pool.Release(enemy);
    }

    void Update()
    {
        if(Time.frameCount % 120 == 1)
        {
            //SpawnOnIsland();
            //SpawnEnemy(GetRandomPosOnScreen());
            //SpawnEnemyInCircle(1f, Random.Range(6, 12));
        }
    }

    public void SpawnEnemyInCircle(float radius, int count)
    {
        Vector2 playerPos = player.transform.position;

        for(int i = 0; i<count; i++)
        {
            float angle = 1f / count * 2f * Mathf.PI * i;

            Vector2 pos = playerPos + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            SpawnEnemy(pos);
        }
    }

    [Button]
    public async Task SpawnEnemyInSpiral(float radiusMin, float radiusMax, int count, float maxAngle = 1.5f, int delay = 150, float enemyPrewarm = 0.5f)
    {
        SpawingOnSpiral = true;
        
        Vector2 playerPos = player.transform.position;
        float randomAngle = Random.Range(0f, 2f);
        
        for(int i = 0; i<count; i++)
        {
            float normal = 1f / count * i;
            float angle = (randomAngle + normal * maxAngle) * 2f * Mathf.PI;
            float radius = radiusMin + (radiusMax - radiusMin) * normal;

            Vector2 pos = playerPos + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            SpawnEnemy(pos, true, enemyPrewarm);
            await Task.Delay(delay);
        }

        SpawingOnSpiral = false;
    }
    
    
    public void SpawnEnemyInLineY(int count, float normalY = -0.9f)
    {
        for (int i = 0; i < count; i++)
        {
            float normalX = 2f / (count + 1) * (i+1) - 1f;
            Vector2 pos = new Vector2(normalX * screenBounds.x, normalY * screenBounds.y);
            SpawnEnemy(pos);
        }
    }

    public void SpawnEnemyAtRandomPos()
    {
        SpawnEnemy(GetRandomPosOnScreen());
    }

    public void SpawnOnIsland(int angle, float x, float y)
    {
        if (Shoot_GameManager.Instacne.state != Shoot_GameManager.ShootGameState.playing) return;
        Shoot_enemy enemy = enemy_pool.Get();
        enemy.transform.SetParent(gameObject.transform);
        enemy.transform.position = new Vector3(island.position.x, island.position.y, 0f);
        enemy.Init(player, 0.4f, 1f, angle, x, y);

        enemy_list.Add(enemy);
    }

    public void GameOver() {
        foreach(Shoot_enemy enemy in enemy_list)
        {
            enemy.state = enemy_stats.gameOver;
        }
        SpawingOnSpiral = false;
        enemy_pool.Clear();
    }

    public void KillAll()
    {
        for(int i = enemy_list.Count-1; i>=0; i--)
        {
            if(enemy_list[i].state == enemy_stats.despawning) continue;
            DestroyEnemy(enemy_list[i]);
            // enemy_list.RemoveAt(i);
        }
        SpawingOnSpiral = false;
        enemy_pool.Clear();
    }
}
