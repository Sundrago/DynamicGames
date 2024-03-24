using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public enum enemy_stats {prewarm, spawning, ready, despawning, gameOver};

public class Shoot_enemy : MonoBehaviour
{
    [SerializeField]
    private Shoot_ScoreManager score;
    public enemy_stats state;
    [SerializeField]
    private Shoot_GameManager shootGameManager;
    [SerializeField]
    private Boundaries boundaries;
    private Transform player;
    private float velocity;
    private float angle;
    private Vector3 direction;
    private Vector2 screenBounds;
    private AudioManager audioManager;


    private void Start()
    {
        audioManager = AudioManager.Instance;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    public void Init(Transform _player, float _velocity, float prewarm_duration = 0, float prewarm_rotation = -1, float prewarm_x = 0, float prewarm_y = 0)
    {
        player = _player;
        velocity = _velocity;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if(prewarm_rotation == -1)
        {
            state = enemy_stats.spawning;

            spriteRenderer.color = Color.red;
            spriteRenderer.DOFade(0f, 2f + prewarm_duration)
                .SetEase(Ease.InCubic)
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
                    if(shootGameManager.spinMode) direction = - player.position + gameObject.transform.position;
                    else direction = player.position - gameObject.transform.position;
                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    gameObject.transform.DORotate(new Vector3(0f, 0f, angle), 0.5f)
                    .SetEase(Ease.OutCubic);
                    spriteRenderer.DOFade(1f, 0.5f)
                        .SetEase(Ease.InCubic)
                        .OnComplete(() => state = enemy_stats.ready);
                });
        }
    }

    void LateUpdate()
    {
        if (state == enemy_stats.prewarm || state == enemy_stats.gameOver) return;

        if(shootGameManager.spinMode) direction = - player.position + gameObject.transform.position;
        else direction = player.position - gameObject.transform.position;
        
        direction.z = 0f;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        if (shootGameManager.spinMode) {
            if (state == enemy_stats.spawning || state == enemy_stats.ready)
            {
                if (Vector2.Distance(player.position, gameObject.transform.position) < 0.45)
                {
                    audioManager.PlaySFXbyTag(SFX_tag.enemy_dead_spin);
                    KillEnemy();
                    return;
                }
                gameObject.transform.position += direction.normalized * velocity / 2f * Time.deltaTime;

                if (gameObject.transform.position.x < boundaries.left.position.x)
                {
                    gameObject.transform.position = new Vector3(boundaries.left.position.x, gameObject.transform.position.y, 0f);
                }
                else if (gameObject.transform.position.x > boundaries.right.position.x)
                {
                    gameObject.transform.position = new Vector3(boundaries.right.position.x, gameObject.transform.position.y, 0f);
                }

                if (gameObject.transform.position.y > boundaries.top.position.y)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, boundaries.top.position.y, 0f);
                }
                else if (gameObject.transform.position.y < boundaries.btm.position.y)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, boundaries.btm.position.y, 0f);
                }

                return;
            }
        }

        if (state == enemy_stats.ready) {
            if (Vector2.Distance(player.position, gameObject.transform.position) < 0.125f)
            {
                Shoot_GameManager.Instacne.GetAttack();
                KillEnemy();
                return;
            }
        }
        gameObject.transform.position += direction.normalized * velocity * Time.deltaTime;
    }

    public void KillEnemy(float delay = 0)
    {
        if (state == enemy_stats.despawning) return;
        state = enemy_stats.despawning;
        score.AddScore(1);
        if(delay == 0) FXManager.Instance.CreateFX(FXType.SmallExplosion, gameObject.transform);

        Color color = Color.black;
        color.a = 0;

        GetComponent<SpriteRenderer>().DOColor(color, 1f)
            .SetDelay(delay)
            .OnComplete(() => Shoot_Enemy_Manager.Instance.DestroyEnemy(this));
    }
}
