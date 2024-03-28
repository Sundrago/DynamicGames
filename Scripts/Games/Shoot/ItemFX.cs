using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Shoot
{
    public class ItemFX : MonoBehaviour
    {
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private FXType type;
        [SerializeField] private float distance;

        private EnemyManager enemyManager;
        private Transform player;
        private float updateDistance;


        private void Awake()
        {
            enemyManager = EnemyManager.Instance;
            player = GameManager.Instacne.player;
            audioManager = AudioManager.Instance;
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0)
            {
                if (type == FXType.blackhole || type == FXType.Bomb) KillEnemy();
                else if (type == FXType.ShadowBomb) KillPlayer();
            }
        }

        private void OnEnable()
        {
            updateDistance = distance;
            if (type == FXType.Bomb) KillEnemy();
        }

        private void KillEnemy()
        {
            for (var i = enemyManager.enemyControllers.Count - 1; i >= 0; i--)
            {
                var enemyController = enemyManager.enemyControllers[i];
                if (Vector2.Distance(gameObject.transform.position, enemyController.gameObject.transform.position) <
                    distance)
                    if (enemyController.enemyStats != EnemyStats.Despawning)
                    {
                        enemyController.KillEnemy(0.5f);
                        enemyController.transform.DOMove(gameObject.transform.position, Random.Range(1f, 2.5f))
                            .SetEase(Ease.InSine);
                        audioManager.PlaySFXbyTag(SfxTag.enemy_dead_blackHole);
                    }
            }
        }

        private void KillPlayer()
        {
            if (Vector2.Distance(player.position, gameObject.transform.position) < updateDistance)
                GameManager.Instacne.GetAttack();

            updateDistance *= 0.7f;
        }

        // [Button]
        // private void DrawDebugLine()
        // {
        //     var pos = gameObject.transform.position;
        //     pos.x += distance;
        //     Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);
        //
        //     pos = gameObject.transform.position;
        //     pos.x -= distance;
        //     Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);
        //
        //     pos = gameObject.transform.position;
        //     pos.y += distance;
        //     Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);
        //
        //     pos = gameObject.transform.position;
        //     pos.y -= distance;
        //     Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);
        // }
    }
}