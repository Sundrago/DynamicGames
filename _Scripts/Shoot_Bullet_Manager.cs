using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum BulletType { normalV1, normalV2, normalV3, }

[Serializable]
public class BulletInfo
{
    public BulletType type;
    [PreviewField(Alignment = ObjectFieldAlignment.Center)]
    public Sprite sprite;

    [VerticalGroup("value")]
    public int points;
    [VerticalGroup("value")]
    public float velocity;
}

public class Shoot_Bullet_Manager : MonoBehaviour
{
    [TableList(ShowIndexLabels = true)]
    public List<BulletInfo> bulletInfos = new List<BulletInfo>();

    [SerializeField] Boundaries island_Boundaries;
    [SerializeField] GameObject island;
    [SerializeField] ShootScoreManager score;
    [SerializeField] ParticleSystem _FX_islandHit;
    [SerializeField] Shoot_bullet _bullet;

    [SerializeField] int defaultCapacity, maxCapacity;
    private ObjectPool<Shoot_bullet> bullet_pool;
    private ObjectPool<ParticleSystem> fx_pool;
    private List<Shoot_bullet> bullets;
    private Vector2 screenBounds;

    private void Start()
    {
        bullet_pool = new ObjectPool<Shoot_bullet>(() =>
        {
            Shoot_bullet bullet = Instantiate(_bullet);
            bullets.Add(bullet);
            return bullet;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            bullets.Remove(bullet);
            Destroy(bullet.gameObject);
        }, false, defaultCapacity, maxCapacity);

        fx_pool = new ObjectPool<ParticleSystem>(() =>
        {
            return Instantiate(_FX_islandHit);
        }, fx =>
        {
            fx.gameObject.SetActive(true);
        }, fx =>
        {
            fx.gameObject.SetActive(false);
        }, fx =>
        {
            Destroy(fx);
        }, false, defaultCapacity, maxCapacity);

        bullets = new List<Shoot_bullet>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

    }

    public void SpawnBullet(BulletType type, Vector2 _position, Vector2 _direction)
    {
        Shoot_bullet bullet = bullet_pool.Get();
        _direction.Normalize();
        bullet.Init(bulletInfos[(int)type].sprite, type, bulletInfos[(int)type].points, new Vector3(_direction.x, _direction.y, 0), bulletInfos[(int)type].velocity);
        bullet.gameObject.transform.SetParent(gameObject.transform);
        bullet.gameObject.transform.position = _position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    private void KillBullet(Shoot_bullet bullet)
    {
        bullet_pool.Release(bullet);
    }

    public void KillParticleFX(ParticleSystem fx)
    {
        fx_pool.Release(fx);
    }

    void Update()
    {
        for(int i = bullets.Count - 1; i >= 0; i--)
        {
            Shoot_bullet bullet = bullets[i];
            if (bullet == null) continue;
            if (!bullet.gameObject.activeSelf) continue;

            bullet.gameObject.transform.position += bullet.direction * bullet.velocity * Time.deltaTime;

            //hit island
            if (bullet.gameObject.transform.position.x > island_Boundaries.left.position.x &&
            bullet.gameObject.transform.position.x < island_Boundaries.right.position.x &&
            bullet.gameObject.transform.position.y > island_Boundaries.btm.position.y &&
            bullet.gameObject.transform.position.y < island_Boundaries.top.position.y)
            {
                island.transform.localScale = new Vector3(1f, 1f, 1f);
                if (DOTween.IsTweening(island.transform)) DOTween.Kill(island.transform);
                island.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.25f);
                score.AddScore(bullet.points);

                ParticleSystem fx = fx_pool.Get();
                fx.transform.SetParent(gameObject.transform);
                fx.gameObject.transform.position = bullet.gameObject.transform.position;
                fx.Play();

                KillBullet(bullet);
                return;
            }

            //touches boundaries
            if (bullet.gameObject.transform.position.x < screenBounds.x * -1)
            {
                bullet.gameObject.transform.position = new Vector3(screenBounds.x * -1, bullet.gameObject.transform.position.y, 0f);
                bullet.direction = new Vector3(bullet.direction.x * -1f, bullet.direction.y, 0f);
                float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
                bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                ++bullet.bounceCount;
            }
            else if (bullet.gameObject.transform.position.x > screenBounds.x)
            {
                bullet.gameObject.transform.position = new Vector3(screenBounds.x, bullet.gameObject.transform.position.y, 0f);
                bullet.direction = new Vector3(bullet.direction.x * -1f, bullet.direction.y, 0f);
                float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
                bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                ++bullet.bounceCount;
            }
            else if (bullet.gameObject.transform.position.y > screenBounds.y)
            {
                bullet.gameObject.transform.position = new Vector3(bullet.gameObject.transform.position.x, screenBounds.y, 0f);
                bullet.direction = new Vector3(bullet.direction.x, bullet.direction.y * -1f, 0f);
                float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
                bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                ++bullet.bounceCount;
            }
            else if (bullet.gameObject.transform.position.y < screenBounds.y * -1)
            {
                bullet.gameObject.transform.position = new Vector3(bullet.gameObject.transform.position.x, screenBounds.y * -1, 0f);
                bullet.direction = new Vector3(bullet.direction.x, bullet.direction.y * -1f, 0f);
                float angle = Mathf.Atan2(bullet.direction.y, bullet.direction.x) * Mathf.Rad2Deg;
                bullet.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                ++bullet.bounceCount;
            }

            if (bullet.bounceCount > 2 || bullet.startTime +5f < Time.time) KillBullet(bullet);
        }
    }
}

