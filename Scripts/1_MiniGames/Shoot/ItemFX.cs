using DG.Tweening;
using DynamicGames.System;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Responsible for handling visual effects related to items in a shooting mini-game.
    /// </summary>
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
                if (type == FXType.Blackhole || type == FXType.Bomb) KillEnemy();
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
                        audioManager.PlaySfxByTag(SfxTag.EnemyDeadBlackHole);
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
        // private void DrawDebugLineBoundaries()
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