using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shoot_item : MonoBehaviour
{
    [SerializeField] Boundaries boundaries;
    [SerializeField] GameObject item, item_holder, FX_itemHit;
    [SerializeField] Transform player;

    [SerializeField] float velocity = 0.2f;
    [SerializeField] Vector2 vec;
    [SerializeField] float width = 2f;

    private List<Shoot_item_obj> Shoot_Items = new List<Shoot_item_obj>();

    void Start()
    {
        GameObject new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        Shoot_item_obj obj = new Shoot_item_obj(new_item.transform, "");
        new_item.SetActive(true);
        Shoot_Items.Add(obj);

        new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        obj = new Shoot_item_obj(new_item.transform, "");
        new_item.SetActive(true);
        Shoot_Items.Add(obj);

        new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        obj = new Shoot_item_obj(new_item.transform, "");
        new_item.SetActive(true);
        Shoot_Items.Add(obj);

        new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        obj = new Shoot_item_obj(new_item.transform, "");
        new_item.SetActive(true);
        Shoot_Items.Add(obj);

        new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        obj = new Shoot_item_obj(new_item.transform, "");
        new_item.SetActive(true);
        Shoot_Items.Add(obj);
    }

    private void SpawnItem()
    {
        GameObject new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = Vector3.zero;
        Shoot_item_obj obj = new Shoot_item_obj(new_item.transform, "");
        new_item.transform.DOScale(new Vector3(0, 0, 0), 1f)
            .From();
        new_item.SetActive(true);
        Shoot_Items.Add(obj);
    }

    void Update()
    {
        if (Time.frameCount % 300 == 1)
        {
            SpawnItem();
        }

        //Move and bounce wall
        for (int i = Shoot_Items.Count - 1; i>=0; i--)
        {
            Shoot_item_obj obj = Shoot_Items[i];

            if (obj == null) continue;

            obj.transform.Translate(obj.velocity * Time.deltaTime * obj.vec);

            if (obj.transform.position.x < boundaries.left.position.x)
            {
                obj.transform.position = new Vector3(boundaries.left.position.x, obj.transform.position.y, 0f);
                obj.vec = new Vector3(obj.vec.x * -1f, obj.vec.y, 0f);
            }
            else if (obj.transform.position.x > boundaries.right.position.x)
            {
                obj.transform.position = new Vector3(boundaries.right.position.x, obj.transform.position.y, 0f);
                obj.vec = new Vector3(obj.vec.x * -1f, obj.vec.y, 0f);
            }
            if (obj.transform.position.y > boundaries.top.position.y)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, boundaries.top.position.y, 0f);
                obj.vec = new Vector3(obj.vec.x, obj.vec.y * -1f, 0f);
            }
            else if (obj.transform.position.y < boundaries.btm.position.y)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, boundaries.btm.position.y, 0f);
                obj.vec = new Vector3(obj.vec.x, obj.vec.y * -1f, 0f);
            }

            if(Vector2.Distance(player.position, obj.transform.position) < 0.25f)
            {
                print(obj.type);
                Shoot_Items.Remove(obj);

                obj.transform.DOScale(0f, 0.25f)
                    .SetEase(Ease.InOutElastic)
                    .OnComplete(() => { Destroy(obj.transform.gameObject); });

                FXManager.Instance.CreateFX(FXType.Bomb, obj.transform);
            }
        }
    }

    class Shoot_item_obj
    {
        public Transform transform;
        public Vector3 vec;
        public string type;
        public float velocity;

        public Shoot_item_obj(Transform _transform, string _type = "", float _velocity = 0.2f)
        {
            transform = _transform;
            vec = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            type = _type;
            velocity = _velocity;
        }
    }
}
