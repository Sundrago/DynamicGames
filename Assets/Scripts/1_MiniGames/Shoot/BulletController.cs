using DynamicGames.System;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Controls the behavior of a bullet in shooting mini-game.
    /// </summary>
    public class BulletController : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private EnemyManager enemyManager;
        
        private int bounceCount;
        private Vector3 direction;
        private GameObject fx;
        private BulletInfo info = new();
        private float velocity, radius, startTime;

        public int Points { get; private set; }

        private void Update()
        {
            UpdatePositionAndCheckBoundaries();
            CheckForTimeOrBounceExpiry();
            HandleEnemyEncounters();
        }

        private void UpdatePositionAndCheckBoundaries()
        {
            gameObject.transform.position += direction * velocity * Time.deltaTime;
            var currentPosition = gameObject.transform.position;

            if (IsInsideIsland(currentPosition, bulletManager.islandBoundaries))
                bulletManager.IslandHit(Points, this);
            else
                UpdateBoundsAndBounce();
        }

        private void CheckForTimeOrBounceExpiry()
        {
            if (bounceCount > bulletManager.bounceCount || startTime + 5f < Time.time) bulletManager.KillBullet(this);
        }

        private void UpdateBoundsAndBounce()
        {
            var x = gameObject.transform.position.x;
            var y = gameObject.transform.position.y;

            if (x < bulletManager.screenBounds.x * -1)
            {
                gameObject.transform.position = new Vector3(bulletManager.screenBounds.x * -1, y, 0f);
                BoundaryBounce(-1f, 1f);
            }
            else if (x > bulletManager.screenBounds.x)
            {
                gameObject.transform.position = new Vector3(bulletManager.screenBounds.x, y, 0f);
                BoundaryBounce(-1f, 1f);
            }
            else if (y > bulletManager.screenBounds.y)
            {
                gameObject.transform.position = new Vector3(x, bulletManager.screenBounds.y, 0f);
                BoundaryBounce(1f, -1f);
            }
            else if (y < bulletManager.screenBounds.y * -1)
            {
                gameObject.transform.position = new Vector3(x, bulletManager.screenBounds.y * -1, 0f);
                BoundaryBounce(1f, -1f);
            }
        }

        private void BoundaryBounce(float xInvert, float yInvert)
        {
            direction = new Vector3(direction.x * xInvert, direction.y * yInvert, 0f);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            ++bounceCount;
        }

        private bool IsInsideIsland(Vector3 currentPosition, Boundaries boundaries)
        {
            return currentPosition.x > boundaries.left.position.x &&
                   currentPosition.x < boundaries.right.position.x &&
                   currentPosition.y > boundaries.btm.position.y &&
                   currentPosition.y < boundaries.top.position.y;
        }

        private void HandleEnemyEncounters()
        {
            for (var enemyIndex = enemyManager.enemyControllers.Count - 1; enemyIndex >= 0; enemyIndex--)
                HandleSingleEnemyEncounter(enemyManager.enemyControllers[enemyIndex], radius);
        }

        private void HandleSingleEnemyEncounter(EnemyController enemyController, float radius)
        {
            if (Vector2.Distance(gameObject.transform.position, enemyController.gameObject.transform.position) <
                radius)
                if (enemyController.enemyStats == EnemyStats.Ready)
                {
                    enemyController.KillEnemy();
                    bulletManager.KillBullet(this);
                    FXManager.Instance.CreateFX(FXType.SmallExplosion, transform);
                    AudioManager.Instance.PlaySfxByTag(SfxTag.EnemyDeadExplosion);
                }
        }

        public void Init(BulletInfo _info, Vector2 _direction, Vector2 _position)
        {
            if (info == _info)
            {
                startTime = Time.time;
                bounceCount = 0;
                direction = _direction;
                return;
            }

            info = _info;
            InitKillFX();
            InitBulletProperties(_direction);
            gameObject.SetActive(true);
        }

        public void SetPositionAndOrientation(Vector2 _direction, Vector2 _position)
        {
            gameObject.transform.SetParent(bulletManager.transform);
            gameObject.transform.position = _position;
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            audioManager.PlaySfxByTag(info.sfx);
        }

        private void InitKillFX()
        {
            if (fx != null) FXManager.Instance.KillFX(fx.GetComponent<FXController>());

            gameObject.GetComponent<SpriteRenderer>().sprite = info.sprite;
            if (info.fx != FXType.Empty)
            {
                fx = FXManager.Instance.CreateFX(info.fx, gameObject.transform);
                fx.gameObject.transform.SetParent(gameObject.transform, true);
                fx.transform.localPosition = Vector3.zero;
                fx.transform.localEulerAngles = new Vector3(0, -90, 0);
            }
            else
            {
                fx = null;
            }
        }

        private void InitBulletProperties(Vector2 _direction)
        {
            Points = info.points;
            direction = _direction;
            velocity = info.velocity;
            radius = info.radius;
            startTime = Time.time;
            bounceCount = 0;
        }
    }
}