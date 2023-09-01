using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shoot_item : MonoBehaviour
{
    [SerializeField] Boundaries boundaries;
    [SerializeField] GameObject item, item_holder, FX_itemHit;
    [SerializeField] Transform player;
    [SerializeField] Shoot_Bullet_Manager bullet_Manager;

    [SerializeField] float velocity = 0.2f;
    [SerializeField] Vector2 vec;
    [SerializeField] float width = 2f;
    [SerializeField] Sprite[] item_imgs;
    [SerializeField] Shoot_GameManager gameManager;


    private Vector2 screenBounds;
    private List<Shoot_item_obj> Shoot_Items = new List<Shoot_item_obj>();

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
    }

    private void SpawnItem()
    {
        if (Shoot_Items.Count >= 4) return;

        GameObject new_item = Instantiate(item, item_holder.transform);
        new_item.transform.localPosition = GetRandomPosOnScreen();

        int rnd = Random.Range(0, 4);

        if (rnd == 1 && gameManager.shield == null) rnd = 0;
        else if (rnd == 2)
        {
            float chance = bullet_Manager.bounceCount * 0.25f;
            if (Random.Range(0f, 1f) < chance) rnd = 0;
        }

        Shoot_item_obj obj = new Shoot_item_obj(new_item.transform, (itemType)rnd);
        new_item.GetComponent<SpriteRenderer>().sprite = item_imgs[rnd];

        new_item.transform.DOScale(new Vector3(0, 0, 0), 1f)
            .From();
        new_item.SetActive(true);
        Shoot_Items.Add(obj);
    }

    void Update()
    {
        if (Time.frameCount % 100 == 0)
        {
            SpawnItem();
        }

        if (gameManager.state != Shoot_GameManager.ShootGameState.playing) return;

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

            if(Vector2.Distance(player.position, obj.transform.position) < 0.3)
            {
                print(obj.type);
                Shoot_Items.Remove(obj);

                obj.transform.DOScale(0f, 0.25f)
                    .SetEase(Ease.InOutElastic)
                    .OnComplete(() => { Destroy(obj.transform.gameObject); });

                GotItem(obj);

                FXManager.Instance.CreateFX(FXType.ItemHit, obj.transform);
            }
        }
    }

    private Vector2 GetRandomPosOnScreen()
    {
        return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
    }

    private void GotItem(Shoot_item_obj obj)
    {
        itemType type = obj.type;

        switch (type)
        {
            case itemType.weapon:
                bullet_Manager.UpgradeBullet();
                break;
            case itemType.shield:
                gameManager.GetShield();
                break;
            case itemType.bounce:
                bullet_Manager.bounceCount += 1;
                if (bullet_Manager.bounceCount > 3) bullet_Manager.bounceCount = 3;
                break;
            case itemType.blackHole:
                FXManager.Instance.CreateFX(FXType.blackhole, obj.transform);
                break;
        }
    }

    class Shoot_item_obj
    {
        public Transform transform;
        public Vector3 vec;
        public itemType type;
        public float velocity;

        public Shoot_item_obj(Transform _transform, itemType _type, float _velocity = 0.2f)
        {
            transform = _transform;
            vec = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0f);
            type = _type;
            velocity = _velocity;
        }
    }

    public enum itemType { weapon, shield, bounce, blackHole }
}
