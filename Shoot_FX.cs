using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Shoot_FX : MonoBehaviour
{
    [SerializeField] FXType type;
    [SerializeField] float distance;

    private Shoot_Enemy_Manager enemy_Manager;
    

    private void OnEnable()
    {
        enemy_Manager = Shoot_Enemy_Manager.Instance;
    }

    public void KillEnemyIfInDistance()
    {
        print(enemy_Manager.enemy_list.Count);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + Vector3.one * distance, Color.red, 3f);
        for (int i = enemy_Manager.enemy_list.Count - 1; i>=0; i--)
        {
            Shoot_enemy enemy = enemy_Manager.enemy_list[i];
            Debug.DrawLine(gameObject.transform.position, enemy.transform.position, Color.blue, 3f);

            if (Vector3.Distance(gameObject.transform.position, enemy.gameObject.transform.position) < distance)
            {
                if(enemy.state != enemy_stats.despawning) enemy.KillEnemy();
            }
        }
    }

    private void Update()
    {
        if(Time.frameCount % 10 == 0)
        {
            KillEnemyIfInDistance();
        }
    }

    [Button]
    void DrawDebugLine()
    {
        Vector3 pos = gameObject.transform.position;
        pos.x += distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 5f);

        pos = gameObject.transform.position;
        pos.x -= distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 5f);

        pos = gameObject.transform.position;
        pos.y += distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 5f);

        pos = gameObject.transform.position;
        pos.y -= distance;
        Debug.DrawLine(gameObject.transform.position, pos, Color.red, 5f);
    }
}
