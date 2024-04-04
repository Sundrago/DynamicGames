using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DynamicGames.System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages the spawning, upgrading, and killing of bullets in the Shoot mini-game.
    /// </summary>
    public class BulletManager : MonoBehaviour
    {
        [Header("Managers and Controllers")] [SerializeField]
        private GameManager gameManager;

        [SerializeField] private InputManager inputManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private BulletController bulletController;
        [SerializeField] private EnemyManager enemyManager;

        [Header("Game Components")] [SerializeField]
        public Boundaries islandBoundaries;

        [SerializeField] public int currentBulletObj;
        [SerializeField] private GameObject island;
        [SerializeField] private ParticleSystem islandHifFX;

        [Header("Game Status")] [TableList(ShowIndexLabels = true)]
        public List<BulletInfo> bulletInfos = new();

        [SerializeField] private Transform player;
        [SerializeField] private int defaultCapacity, maxCapacity;

        public int bounceCount;
        private ObjectPool<BulletController> bulletObjectPool;
        private List<BulletController> bullets;
        private ObjectPool<ParticleSystem> fxObjectPool;
        public Vector2 screenBounds { get; private set; }

        public static BulletManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            bullets = new List<BulletController>();
            bulletObjectPool = InitializeBulletPool();
            fxObjectPool = InitializeFxPool();

            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
        }

        private ObjectPool<BulletController> InitializeBulletPool()
        {
            return new ObjectPool<BulletController>(() =>
                {
                    var bulletController = Instantiate(this.bulletController);
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

        public void KillBullet(BulletController bulletController)
        {
            if (bulletController.gameObject.activeSelf) bulletObjectPool.Release(bulletController);
        }

        public void KillParticleFX(ParticleSystem fx)
        {
            fxObjectPool.Release(fx);
        }

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

        public void IslandHit(int point, BulletController bulletController)
        {
            island.transform.localScale = new Vector3(1f, 1f, 1f);
            island.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.25f);
            scoreManager.AddScore(bulletController.points);

            var fx = fxObjectPool.Get();
            fx.transform.SetParent(gameObject.transform);
            fx.gameObject.transform.position = bulletController.gameObject.transform.position;
            fx.Play();

            KillBullet(bulletController);
        }
    }

    [Serializable]
    public class BulletInfo
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite sprite;

        [VerticalGroup("fx")] public FXType fx;
        [VerticalGroup("fx")] public SfxTag sfx = SfxTag.ShootTypeA;

        [VerticalGroup("value")] public int points = 1;
        [VerticalGroup("value")] public float velocity;
        [VerticalGroup("value")] public float radius;
        [VerticalGroup("value")] public int intervalInMeleSec = 150;
    }
}