using System;
using UnityEngine;

public class Shoot_bullet : MonoBehaviour
{
    public Vector3 direction;
    public float velocity;
    public int points;
    public int bounceCount;
    public float startTime;
    public float radius;

    public GameObject fx = null;
    public BulletInfo info = new BulletInfo();

    public void Init(BulletInfo _info, Vector2 _direction)
    {
        if (info == _info)
        {
            startTime = Time.time;
            bounceCount = 0;
            direction = _direction;
            return;
        }
        info = _info;
        
        if (fx != null)
        {
            FXManager.Instance.KillFX(fx.GetComponent<FX>());
        }
        
        gameObject.GetComponent<SpriteRenderer>().sprite = info.sprite;
        if (info.fx != FXType.empty)
        {
            fx = FXManager.Instance.CreateFX(info.fx, gameObject.transform);
            fx.gameObject.transform.SetParent(gameObject.transform, true);
            fx.transform.localPosition = Vector3.zero;
            fx.transform.localEulerAngles = new Vector3(0, -90, 0);
        }
        else fx = null;

        points = info.points;
        direction = _direction;
        velocity = info.velocity;
        radius = info.radius;

        startTime = Time.time;
        bounceCount = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        gameObject.transform.position += direction * velocity * Time.deltaTime;

        //hit island
        if (gameObject.transform.position.x > Shoot_Bullet_Manager.Instance.island_Boundaries.left.position.x &&
        gameObject.transform.position.x < Shoot_Bullet_Manager.Instance.island_Boundaries.right.position.x &&
        gameObject.transform.position.y > Shoot_Bullet_Manager.Instance.island_Boundaries.btm.position.y &&
        gameObject.transform.position.y < Shoot_Bullet_Manager.Instance.island_Boundaries.top.position.y)
        {
            Shoot_Bullet_Manager.Instance.IslandHit(points, this);
            return;
        }

        //touches boundaries
        if (gameObject.transform.position.x < Shoot_Bullet_Manager.Instance.screenBounds.x * -1)
        {
            gameObject.transform.position = new Vector3(Shoot_Bullet_Manager.Instance.screenBounds.x * -1, gameObject.transform.position.y, 0f);
            direction = new Vector3(direction.x * -1f, direction.y, 0f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            ++bounceCount;
        }
        else if (gameObject.transform.position.x > Shoot_Bullet_Manager.Instance.screenBounds.x)
        {
            gameObject.transform.position = new Vector3(Shoot_Bullet_Manager.Instance.screenBounds.x, gameObject.transform.position.y, 0f);
            direction = new Vector3(direction.x * -1f, direction.y, 0f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            ++bounceCount;
        }
        else if (gameObject.transform.position.y > Shoot_Bullet_Manager.Instance.screenBounds.y)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Shoot_Bullet_Manager.Instance.screenBounds.y, 0f);
            direction = new Vector3(direction.x, direction.y * -1f, 0f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            ++bounceCount;
        }
        else if (gameObject.transform.position.y < Shoot_Bullet_Manager.Instance.screenBounds.y * -1)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Shoot_Bullet_Manager.Instance.screenBounds.y * -1, 0f);
            direction = new Vector3(direction.x, direction.y * -1f, 0f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            ++bounceCount;
        }

        if (bounceCount > Shoot_Bullet_Manager.Instance.bounceCount || startTime + 5f < Time.time)
        {
            Shoot_Bullet_Manager.Instance.KillBullet(this);
            return;
        }


        for(int j = Shoot_Enemy_Manager.Instance.enemy_list.Count-1; j >= 0; j--)
        {
            Shoot_enemy enemy = Shoot_Enemy_Manager.Instance.enemy_list[j];
            if(Vector2.Distance(gameObject.transform.position, enemy.gameObject.transform.position) < radius)
            {
                if (enemy.state != enemy_stats.ready) continue;
                enemy.KillEnemy();
                Shoot_Bullet_Manager.Instance.KillBullet(this);
                FXManager.Instance.CreateFX(FXType.SmallExplosion, transform);
                AudioManager.Instance.PlaySFXbyTag(SFX_tag.enemy_dead_explostion);
                break;
            }
        }
    }
}