using UnityEngine;

public class Shoot_bullet : MonoBehaviour
{
    public Vector3 direction;
    public float velocity;
    public BulletType type;
    public int points;
    public int bounceCount;
    public float startTime;

    public void Init(Sprite _sprite, BulletType _type, int _points, Vector2 _direction, float _velocity)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _sprite;
        type = _type;
        points = _points;
        direction = _direction;
        velocity = _velocity;

        startTime = Time.time;
        bounceCount = 0;
    }
}