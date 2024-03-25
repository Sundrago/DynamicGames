using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Serialization;

public class Shoot_FX : MonoBehaviour
{
    [SerializeField] FXType type;
    [SerializeField] float distance;
    [FormerlySerializedAs("audioCtrl")] [SerializeField]
    private AudioManager audioManager;

    private float updateDistance;

    private Shoot_Enemy_Manager enemy_Manager;
    Transform player;
    

    private void Awake()
    {
        enemy_Manager = Shoot_Enemy_Manager.Instance;
        player = Shoot_GameManager.Instacne.player;
        audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        updateDistance = distance;
        if (type == FXType.Bomb) KillEnemyIfInDistance();
    }

    private void KillEnemyIfInDistance()
    {
        for (int i = enemy_Manager.enemy_list.Count - 1; i>=0; i--)
        {
            Shoot_enemy enemy = enemy_Manager.enemy_list[i];
            if (Vector2.Distance(gameObject.transform.position, enemy.gameObject.transform.position) < distance)
            {
                if (enemy.state != enemy_stats.despawning)
                {
                    enemy.KillEnemy(0.5f);
                    enemy.transform.DOMove(gameObject.transform.position, Random.Range(1f, 2.5f))
                        .SetEase(Ease.InSine);
                    audioManager.PlaySFXbyTag(SfxTag.enemy_dead_blackHole);
                }
            }
        }
    }

    private void KillPlayerIfInDistance()
    {
        if(Vector2.Distance(player.position, gameObject.transform.position) < updateDistance)
        {
            Shoot_GameManager.Instacne.GetAttack();
        }

        updateDistance *= 0.7f;
    }
        
    private void Update()
    {
        if(Time.frameCount % 10 == 0)
        {
            if (type == FXType.blackhole || type == FXType.Bomb) KillEnemyIfInDistance();
            else if(type == FXType.ShadowBomb) KillPlayerIfInDistance();
        }
    }

    [Button]
    void DrawDebugLine()
    {
        Vector3 pos = gameObject.transform.position;
        pos.x += distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);

        pos = gameObject.transform.position;
        pos.x -= distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);

        pos = gameObject.transform.position;
        pos.y += distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);

        pos = gameObject.transform.position;
        pos.y -= distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 2f);
    }
}
