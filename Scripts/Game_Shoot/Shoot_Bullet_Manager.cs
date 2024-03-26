using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine.Serialization;

[Serializable]
public class BulletInfo
{
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite sprite;
    
    [VerticalGroup("fx")]
    public FXType fx;
    [VerticalGroup("fx")]
    public SfxTag sfx = SfxTag.shootA;
    
    [VerticalGroup("value")]
    public int points = 1;
    [VerticalGroup("value")]
    public float velocity;
    [VerticalGroup("value")]
    public float radius;
    [VerticalGroup("value")]
    public int intervalInMeleSec = 150;
    
}

public class Shoot_Bullet_Manager : MonoBehaviour
{
    [TableList(ShowIndexLabels = true)]
    public List<BulletInfo> bulletInfos = new List<BulletInfo>();

    [SerializeField] public Boundaries island_Boundaries;
    [SerializeField] GameObject island;
    [SerializeField] Shoot_ScoreManager score;
    [SerializeField] ParticleSystem _FX_islandHit;
    [SerializeField] Shoot_bullet _bullet;
    [SerializeField] public int currentBullet = 0;
    [SerializeField] Shoot_joystick joystick;
    [SerializeField] Shoot_GameManager gameManager;
    [SerializeField] Transform player;
    [FormerlySerializedAs("audioCtrl")] [SerializeField]
    private AudioManager audioManager;

    [SerializeField] int defaultCapacity, maxCapacity;
    private ObjectPool<Shoot_bullet> bullet_pool;
    private ObjectPool<ParticleSystem> fx_pool;
    private List<Shoot_bullet> bullets;
    public Vector2 screenBounds;
    private Shoot_Enemy_Manager enemy_Manager;
    public int bounceCount = 0;
    public static Shoot_Bullet_Manager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enemy_Manager = Shoot_Enemy_Manager.Instance;

        bullet_pool = new ObjectPool<Shoot_bullet>(() =>
        {
            Shoot_bullet bullet = Instantiate(_bullet);
            bullets.Add(bullet);
            return bullet;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            bullets.Remove(bullet);
            Destroy(bullet.gameObject);
        }, true, defaultCapacity, maxCapacity);

        fx_pool = new ObjectPool<ParticleSystem>(() =>
        {
            return Instantiate(_FX_islandHit);
        }, fx =>
        {
            fx.gameObject.SetActive(true);
        }, fx =>
        {
            fx.gameObject.SetActive(false);
        }, fx =>
        {
            Destroy(fx);
        }, false, defaultCapacity, maxCapacity);

        bullets = new List<Shoot_bullet>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

    }

    public void Restart()
    {
        bounceCount = 0;
        currentBullet = 0;
        // bullet_pool.Clear();
        
        for(int i = bullets.Count-1; i>=0; i--)
        {
            KillBullet(bullets[i]);
        }
    }

    public void SepawnBullet(Vector2 _position, Vector2 _direction)
    {
        Shoot_bullet bullet = bullet_pool.Get();
        bullets.Add(bullet);
        _direction.Normalize();
        bullet.Init(bulletInfos[currentBullet], new Vector3(_direction.x, _direction.y, 0));
        bullet.gameObject.transform.SetParent(gameObject.transform);
        bullet.gameObject.transform.position = _position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        audioManager.PlaySFXbyTag(bulletInfos[currentBullet].sfx);
    }

    public void UpgradeBullet()
    {
        currentBullet += 1;
        if(currentBullet >= bulletInfos.Count)
        {
            currentBullet = bulletInfos.Count - 1;
        }
    }

    public void KillBullet(Shoot_bullet bullet)
    {
        // if(bullet.fx != null && bullet.fx.activeSelf) FXManager.Instance.KillFX(bullet.fx.GetComponent<FX>());
        if(bullet.gameObject.activeSelf) bullet_pool.Release(bullet);
    }

    public void KillParticleFX(ParticleSystem fx)
    {
        fx_pool.Release(fx);
    }

    // void LateUpdate()
    // {
    //     for(int i = bullets.Count - 1; i >= 0; i--)
    //     {
    //         Shoot_bullet bullet = bullets[i];
    //         if (bullet == null) continue;
    //         if (!bullet.gameObject.activeSelf) continue;
    //
    //         bullet.gameObject.transform.position += bullet.direction * bullet.velocity * Time.deltaTime;
    //
    //         //hit island
    //         if (bullet.gameObject.transform.position.x > island_Boundaries.left.position.x &&
    //         bullet.gameObject.transform.position.x < island_Boundaries.right.position.x &&
    //         bullet.gameObject.transform.position.y > island_Boundaries.btm.position.y &&
    //         bullet.gameObject.transform.position.y < island_Boundaries.top.position.y)
    //         {
    //             island.transform.localScale = new Vector3(1f, 1f, 1f);
    //             island.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f);
    //             score.AddScore(bullet.points);
    //
    //             ParticleSystem fx = fx_pool.Get();
    //             fx.transform.SetParent(gameObject.transform);
    //             fx.gameObject.transform.position = bullet.gameObject.transform.position;
    //             fx.Play();
    //
    //             KillBullet(bullet);
    //             continue;
    //         }
    //
    //         //touches boundaries
    //         if (bullet.gameObject.transform.position.x < screenBounds.x * -1)
    //         {
    //             bullet.gameObject.transform.position = new Vector3(screenBounds.x * -1, bullet.gameObject.transform.position.y, 0f);
    //             bullet.direction = new Vector3(bullet.direction.x * -1f, bullet.direction.y, 0f);
    //             float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
    //             bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    //             ++bullet.bounceCount;
    //         }
    //         else if (bullet.gameObject.transform.position.x > screenBounds.x)
    //         {
    //             bullet.gameObject.transform.position = new Vector3(screenBounds.x, bullet.gameObject.transform.position.y, 0f);
    //             bullet.direction = new Vector3(bullet.direction.x * -1f, bullet.direction.y, 0f);
    //             float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
    //             bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    //             ++bullet.bounceCount;
    //         }
    //         else if (bullet.gameObject.transform.position.y > screenBounds.y)
    //         {
    //             bullet.gameObject.transform.position = new Vector3(bullet.gameObject.transform.position.x, screenBounds.y, 0f);
    //             bullet.direction = new Vector3(bullet.direction.x, bullet.direction.y * -1f, 0f);
    //             float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
    //             bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    //             ++bullet.bounceCount;
    //         }
    //         else if (bullet.gameObject.transform.position.y < screenBounds.y * -1)
    //         {
    //             bullet.gameObject.transform.position = new Vector3(bullet.gameObject.transform.position.x, screenBounds.y * -1, 0f);
    //             bullet.direction = new Vector3(bullet.direction.x, bullet.direction.y * -1f, 0f);
    //             float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
    //             bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    //             ++bullet.bounceCount;
    //         }
    //
    //         if (bullet.bounceCount > bounceCount || bullet.startTime + 5f < Time.time)
    //         {
    //             KillBullet(bullet);
    //             continue;
    //         }
    //
    //         //hit enemy
    //         List<Shoot_enemy> enemy_list = enemy_Manager.enemy_list;
    //
    //         for(int j = enemy_list.Count-1; j >= 0; j--)
    //         {
    //             Shoot_enemy enemy = enemy_list[j];
    //             if(Vector2.Distance(bullet.gameObject.transform.position, enemy.gameObject.transform.position) < bullet.radius)
    //             {
    //                 if (enemy.state != enemy_stats.ready) continue;
    //                 enemy.KillEnemy();
    //                 KillBullet(bullet);
    //                 FXManager.Instance.CreateFX(FXType.SmallExplosion, bullet.transform);
    //                 audioCtrl.PlaySFXbyTag(SFX_tag.enemy_dead_explostion);
    //                 break;
    //             }
    //         }
    //     }
    // }

    private bool StartSpawnBulletTimer_on = false;
    public async Task SpawnBulletTimer()
    {
        if (joystick.vecNormal != Vector3.zero)
        {
            SepawnBullet(player.transform.position, joystick.vecNormal);
        }

        if(gameManager.state == Shoot_GameManager.ShootGameState.playing)
        {
            StartSpawnBulletTimer_on = true;
            await Task.Delay(bulletInfos[currentBullet].intervalInMeleSec);
            SpawnBulletTimer();
        }
        else
        {
            StartSpawnBulletTimer_on = false;
        }
    }

    public void StartSpawnBulletTimer()
    {
        if (!StartSpawnBulletTimer_on)
            SpawnBulletTimer();
    }

    public void IslandHit(int point, Shoot_bullet bullet)
    {
        // DOTween.Kill(island.transform);
        island.transform.localScale = new Vector3(1f, 1f, 1f);
        island.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f);
        score.AddScore(bullet.points);

        ParticleSystem fx = fx_pool.Get();
        fx.transform.SetParent(gameObject.transform);
        fx.gameObject.transform.position = bullet.gameObject.transform.position;
        fx.Play();

        KillBullet(bullet);
    }
}

