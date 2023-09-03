using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Shoot_item_prefab : MonoBehaviour
{
    [SerializeField]
    private Transform timer_sclaer_ui;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public Vector3 vec;
    public itemType type;
    public float velocity;

    private float duration;
    private float startTime;

    public void Init(Vector2 pos, itemType _type, Sprite _sprite, float _velocity = 0.2f, float _duration = 0)
    {
        if (_duration == 0) _duration = Random.Range(6f, 11f);
        
        gameObject.transform.localPosition = pos;
        vec = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
        type = _type;
        velocity = _velocity;
        
        spriteRenderer.sprite = _sprite;

        startTime = Time.time;
        duration = _duration;
        GetNormalizedTimer();
    }

    public float GetNormalizedTimer()
    {
        float normalTimer = 1f - Mathf.Clamp(((Time.time - startTime) / duration), 0f, 1f);
        timer_sclaer_ui.localScale= new Vector3(EaseInOutQuad(normalTimer), 1f, 1f);
        return (normalTimer);
        
        float EaseInOutQuad(float x) {
            return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
        }
    }
}
