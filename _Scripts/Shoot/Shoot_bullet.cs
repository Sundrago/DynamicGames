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
    }
}