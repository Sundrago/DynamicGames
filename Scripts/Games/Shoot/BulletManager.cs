using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Games.Shoot
{
    public class BulletManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager gameManager;
        [SerializeField] private InputManager inputManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private BulletObject bulletObject;
        [SerializeField] private EnemyManager enemyManager;
        
        [Header("Game Components")] 
        [SerializeField] public Boundaries islandBoundaries;
        [SerializeField] public int currentBulletObj;
        [SerializeField] private GameObject island;
        [SerializeField] private ParticleSystem islandHifFX;

        [Header("Game Status")] 
        [TableList(ShowIndexLabels = true)] public List<BulletInfo> bulletInfos = new();
        [SerializeField] private Transform player;
        [SerializeField] private int defaultCapacity, maxCapacity;

        public int bounceCount;
        public Vector2 screenBounds { get; private set; }
        private ObjectPool<BulletObject> bulletObjectPool;
        private ObjectPool<ParticleSystem> fxObjectPool;
        private List<BulletObject> bullets;
        
        
        public static BulletManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            bullets = new List<BulletObject>();
            bulletObjectPool = InitializeBulletPool();
            fxObjectPool = InitializeFxPool();

            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
        }

        private ObjectPool<BulletObject> InitializeBulletPool()
        {
            return new ObjectPool<BulletObject>(() =>
                {
                    var bulletController = Instantiate(this.bulletObject);
                    bullets.Add(bulletController);
                    return bulletController;
                },
                bullet => bullet.gameObject.SetActive(true),
                bullet => bullet.gameObject.SetActive(false),
                bullet =>
                {
                    bullets.Remove(bullet);
                    Destroy(bullet.gameObject);
                }, true, defaultCapacity, maxCapacity);
        }

        private ObjectPool<ParticleSystem> InitializeFxPool()
        {
            return new ObjectPool<ParticleSystem>(() => Instantiate(islandHifFX),
                fx => fx.gameObject.SetActive(true),
                fx => fx.gameObject.SetActive(false),
                fx => Destroy(fx), false, defaultCapacity, maxCapacity);
        }
        
        public void Restart()
        {
            bounceCount = 0;
            currentBulletObj = 0;
            for (var i = bullets.Count - 1; i >= 0; i--) KillBullet(bullets[i]);
        }

        // public void SepawnBullet(Vector2 position, Vector2 direction)
        // {
        //     var bulletObject = bullet_pool.Get();
        //     bullets.Add(bulletObject);
        //     
        //     direction = direction.normalized;
        //     bulletObject.Init(bulletInfos[currentBulletObj], new Vector3(direction.x, direction.y, 0), position);
        // }
        
        public void SpawnBullet(Vector2 _position, Vector2 _direction)
        {
            if (inputManager.NormalVector == Vector3.zero) return;
            var bulletObject = bulletObjectPool.Get();
            bullets.Add(bulletObject);
            _direction.Normalize();
            bulletObject.Init(bulletInfos[currentBulletObj], new Vector3(_direction.x, _direction.y, 0), _position);
            bulletObject.SetPositionAndOrientation(_direction, _position);
        }

        public void UpgradeBullet()
        {
            currentBulletObj += 1;
            if (currentBulletObj >= bulletInfos.Count) currentBulletObj = bulletInfos.Count - 1;
        }

        public void KillBullet(BulletObject bulletObject)
        {
            if (bulletObject.gameObject.activeSelf) bulletObjectPool.Release(bulletObject);
        }

        public void KillParticleFX(ParticleSystem fx)
        {
            fxObjectPool.Release(fx);
        }
        
        [Button]
        private async Task SpawnBulletTimer()
        {
            while (gameManager.state == GameManager.ShootGameState.playing)
            {
                await Task.Delay(bulletInfos[currentBulletObj].intervalInMeleSec);
                SpawnBullet(player.transform.position, inputManager.NormalVector);
            }
        }

        public void StartSpawnBulletTimer()
        {
            SpawnBulletTimer();
        }

        public void IslandHit(int point, BulletObject bulletObject)
        {
            island.transform.localScale = new Vector3(1f, 1f, 1f);
            island.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f);
            scoreManager.AddScore(bulletObject.points);

            var fx = fxObjectPool.Get();
            fx.transform.SetParent(gameObject.transform);
            fx.gameObject.transform.position = bulletObject.gameObject.transform.position;
            fx.Play();

            KillBullet(bulletObject);
        }
    }

    [Serializable]
    public class BulletInfo
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite sprite;

        [VerticalGroup("fx")] public FXType fx;
        [VerticalGroup("fx")] public SfxTag sfx = SfxTag.shootA;

        [VerticalGroup("value")] public int points = 1;
        [VerticalGroup("value")] public float velocity;
        [VerticalGroup("value")] public float radius;
        [VerticalGroup("value")] public int intervalInMeleSec = 150;
    }
}