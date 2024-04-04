using Core.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Shoot
{
    public enum EnemyStats
    {
        Prewarm,
        Spawning,
        Ready,
        Despawning,
        GameOver
    }

    public class EnemyController : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private ScoreManager scoreManager;
        [FormerlySerializedAs("gameManagerManager")] [SerializeField] private GameManager gameManager;
        [SerializeField] private AudioManager audioManager;

        [Header("Game components")] 
        [SerializeField] private Boundaries boundaries;

        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [FormerlySerializedAs("state")] public EnemyStats enemyStats;
        private float angle, velocity;
        private Vector3 direction;
        private Transform player;
        private Vector2 screenBounds;
        
        private const float NearDistance = 0.45f;
        private const float AttackDistance = 0.125f;
        
        private void Start()
        {
            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
        }

        private void LateUpdate()
        {
            if (IsStateEnemyStatsPrewarmOrGameOver()) return;

            direction = GetDirection();
            direction.z = 0f;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            HandleEnemyMotion();
        }

        private bool IsStateEnemyStatsPrewarmOrGameOver()
        {
            return (enemyStats == EnemyStats.Prewarm || enemyStats == EnemyStats.GameOver);
        }

        private bool IsStateEnemyStatsSpawningOrReady()
        {
            return enemyStats == EnemyStats.Spawning || enemyStats == EnemyStats.Ready;
        }

        private bool IsInAttackDistance()
        {
            return Vector2.Distance(player.position, gameObject.transform.position) < AttackDistance;
        }
        
        private Vector3 GetDirection()
        {
            return gameManager.spinMode 
                ? -player.position + gameObject.transform.position 
                : player.position - gameObject.transform.position;
        }

        private void HandleEnemyMotion()
        {
            if (gameManager.spinMode && IsStateEnemyStatsSpawningOrReady())
            {
                PerformSpinModeAction();
            }
            else if (IsInAttackDistance() && enemyStats == EnemyStats.Ready) {
                GameManager.Instacne.GetAttack();
                KillEnemy();    
            }
            else
            {
                gameObject.transform.position += direction.normalized * velocity * Time.deltaTime;
            }
        }
        
        private void PerformSpinModeAction()
        {
            if (IsPlayerNear())
            {
                audioManager.PlaySfxByTag(SfxTag.EnemyDeadSpin);
                KillEnemy();
                return;
            }
            
            gameObject.transform.position += direction.normalized * velocity / 2f * Time.deltaTime;
            gameObject.transform.position = EnsurePositionWithinBoundaries(gameObject.transform.position);
        }

        private bool IsPlayerNear()
        {
            return Vector2.Distance(player.position, gameObject.transform.position) < NearDistance;
        }
        
        private Vector3 EnsurePositionWithinBoundaries(Vector3 position)
        {
            if (position.x < boundaries.left.position.x)
                position = new Vector3(boundaries.left.position.x, position.y, 0f);
            else if (position.x > boundaries.right.position.x)
                position = new Vector3(boundaries.right.position.x, position.y, 0f);

            if (position.y > boundaries.top.position.y)
                position = new Vector3(position.x, boundaries.top.position.y, 0f);
            else if (position.y < boundaries.btm.position.y)
                position = new Vector3(position.x, boundaries.btm.position.y, 0f);

            return position;
        }

        public void Init(Transform _player, float _velocity, float prewarmDuration = 0, float prewarmRotation = -1, float prewarmX = 0, float prewarmY = 0)
        {
            player = _player;
            velocity = _velocity;

            if (prewarmRotation == -1)
            {
                InitiateSpawning(spriteRenderer, prewarmDuration);
            }
            else
            {
                PrewarmSettings(prewarmRotation, prewarmX, prewarmY, prewarmDuration);
            }
        }

        private void InitiateSpawning(SpriteRenderer spriteRenderer, float prewarmDuration)
        {
            enemyStats = EnemyStats.Spawning;

            spriteRenderer.color = Color.red;
            spriteRenderer.DOFade(0f, 2f + prewarmDuration)
                .SetEase(Ease.InCubic)
                .From()
                .OnComplete(() => enemyStats = EnemyStats.Ready);
        }

        private void PrewarmSettings(float prewarmRotation, float prewarmX,
            float prewarmY, float prewarmDuration)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            enemyStats = EnemyStats.Prewarm;
            gameObject.transform.localEulerAngles = new Vector3(0f, 0f, prewarmRotation);
            gameObject.transform.DOMoveX(prewarmX, prewarmDuration)
                .SetEase(Ease.OutCubic)
                .SetRelative(true);
            gameObject.transform.DOMoveY(prewarmY, prewarmDuration)
                .SetRelative(true)
                .SetEase(Ease.OutCubic)
                .OnComplete(OnPrewarmFinish);
        }

        private void OnPrewarmFinish()
        {
            if (gameManager.spinMode) direction = -player.position + gameObject.transform.position;
            else direction = player.position - gameObject.transform.position;
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.DORotate(new Vector3(0f, 0f, angle), 0.5f)
                .SetEase(Ease.OutCubic);
            spriteRenderer.DOFade(1f, 0.5f)
                .SetEase(Ease.InCubic)
                .OnComplete(() => enemyStats = EnemyStats.Ready);
        }
        public void KillEnemy(float delay = 0)
        {
            if (enemyStats == EnemyStats.Despawning) return;
            enemyStats = EnemyStats.Despawning;
            scoreManager.AddScore(1);
            if (delay == 0) FXManager.Instance.CreateFX(FXType.SmallExplosion, gameObject.transform);

            var color = Color.black;
            color.a = 0;

            GetComponent<SpriteRenderer>().DOColor(color, 1f)
                .SetDelay(delay)
                .OnComplete(() => EnemyManager.Instance.DestroyEnemy(this));
        }
    }
}