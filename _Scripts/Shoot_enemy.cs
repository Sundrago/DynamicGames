using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum enemy_stats {prewarm, spawning, ready, despawning, gameOver};

public class Shoot_enemy : MonoBehaviour
{
    public enemy_stats state;

    private Transform player;
    private float velocity;
    private float angle;
    private Vector3 direction;

    public void Init(Transform _player, float _velocity, float prewarm_duration = 1.5f, float prewarm_rotation = -1, float prewarm_x = 0, float prewarm_y = 0)
    {
        player = _player;
        velocity = _velocity;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if(prewarm_rotation == -1)
        {
            state = enemy_stats.spawning;

            spriteRenderer.color = Color.red;
            spriteRenderer.DOFade(0f, 2f)
                .From()
                .OnComplete(() => state = enemy_stats.ready);
        } else
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            state = enemy_stats.prewarm;
            gameObject.transform.localEulerAngles = new Vector3(0f, 0f, prewarm_rotation);
            gameObject.transform.DOMoveX(prewarm_x, prewarm_duration)
                .SetEase((Ease.OutCubic))
                .SetRelative(true);
            gameObject.transform.DOMoveY(prewarm_y, prewarm_duration)
                .SetRelative(true)
                .SetEase((Ease.OutCubic))
                .OnComplete(()=> {
                    direction = player.position - gameObject.transform.position;
                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    gameObject.transform.DORotate(new Vector3(0f, 0f, angle), 0.5f)
                    .SetEase(Ease.OutCubic);
                    spriteRenderer.DOFade(1f, 0.5f)
                        .OnComplete(() => state = enemy_stats.ready);
                });
        }
    }

    void Update()
    {
        if (state == enemy_stats.prewarm || state == enemy_stats.gameOver) return;

        direction = player.position - gameObject.transform.position;
        direction.z = 0f;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if (state == enemy_stats.ready)
        {
            if (Vector2.Distance(player.position, gameObject.transform.position) < 0.125f)
            {
                Shoot_GameManager.Instacne.GetAttack();
                KillEnemy();
                return;
            }
            gameObject.transform.position += direction.normalized * velocity * Time.deltaTime;
        }
    }

    public void KillEnemy(float delay = 0)
    {
        if (state == enemy_stats.despawning) return;
        state = enemy_stats.despawning;
        if(delay == 0) FXManager.Instance.CreateFX(FXType.SmallExplosion, gameObject.transform);

        Color color = Color.black;
        color.a = 0;

        GetComponent<SpriteRenderer>().DOColor(color, 1f)
            .SetDelay(delay)
            .OnComplete(() => Shoot_Enemy_Manager.Instance.DestroyEnemy(this));
    }
}
