using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class Shoot_item : MonoBehaviour
{
    [SerializeField] Boundaries boundaries, islandBounaries;
    [SerializeField] private Shoot_item_prefab itemPrefab;
    [SerializeField] Transform player;
    [SerializeField] Shoot_Bullet_Manager bullet_Manager;
    
    [SerializeField] Sprite[] item_imgs;
    [SerializeField] Shoot_GameManager gameManager;
    [FormerlySerializedAs("audioCtrl")] [SerializeField]
    private AudioManager audioManager;

    private Vector2 screenBounds;
    public List<Shoot_item_prefab> items = new List<Shoot_item_prefab>();
    private int totalItemCount = 0;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        KillAll();
    }

    public void SpawnItem()
    {
        if (gameManager.state != Shoot_GameManager.ShootGameState.playing) return;
        if (items.Count >= 3) return;
        // if(gameManager.stage * 2.5f < totalItemCount) return;

        Shoot_item_prefab new_item = Instantiate(itemPrefab, gameObject.transform);
        int rnd = Random.Range(0, 5);
        itemType type = (itemType)rnd;
        
        if (type == itemType.shield && gameManager.shield != null) type = GetRandomSingleItem();
        else if (type == itemType.bounce)
        {
            float chance = bullet_Manager.bounceCount * 0.25f;
            if (Random.Range(0f, 1f) < chance) type = GetRandomSingleItem();
        }
        
        new_item.Init(GetRandomPosOnScreen(), type, item_imgs[(int)type]);
        new_item.transform.DOScale(new Vector3(0, 0, 0), 1f)
            .From();
        new_item.gameObject.SetActive(true);
        items.Add(new_item);

        totalItemCount += 1;
    }
    
    itemType GetRandomSingleItem()
    {
        return (itemType)Random.Range(2, 4);
    }

    public void UpdateItem(Shoot_item_prefab item)
    {
        if (gameManager.state != Shoot_GameManager.ShootGameState.playing) return;
        int rnd = Random.Range(0, 5);

        itemType type = (itemType)rnd;
        if (type == itemType.shield && gameManager.shield != null) type = GetRandomSingleItem();
        else if (type == itemType.bounce)
        {
            float chance = bullet_Manager.bounceCount * 0.25f;
            if (Random.Range(0f, 1f) < chance) type = GetRandomSingleItem();
        }
        
        item.Init(item.transform.localPosition, (itemType)rnd, item_imgs[rnd]);
        item.transform.DOPunchScale(new Vector3(0.03f, 0.03f, 0), 0.5f).SetEase(Ease.InOutQuad);
    }

    void Update()
    {
        if (gameManager.state != Shoot_GameManager.ShootGameState.playing) return;

        for (int i = items.Count - 1; i>=0; i--)
        {
            Shoot_item_prefab obj = items[i];

            //Update Item if Timer ends
            if(obj.GetNormalizedTimer() < 0.01f) UpdateItem(obj);
            
            //Move and bounce on wall
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
            
            //Move Out When On Island
            if (obj.transform.position.x > islandBounaries.left.position.x &&
                obj.transform.position.x < islandBounaries.right.position.x &&
                obj.transform.position.y > islandBounaries.btm.position.y &&
                obj.transform.position.y < islandBounaries.top.position.y)
            {
                if (obj.vec.y > -1.5f)
                {
                    obj.vec.y += -15f * Time.deltaTime;
                }
            }

            //When Player Get near item
            if(Vector2.Distance(player.position, obj.transform.position) < 0.3)
            {
                items.Remove(obj);
                obj.transform.DOScale(0f, 0.25f)
                    .SetEase(Ease.InOutElastic)
                    .OnComplete(() => {
                        Destroy(obj.transform.gameObject);
                    });
                GotItem(obj);
                FXManager.Instance.CreateFX(FXType.ItemHit, obj.transform);
            }
        }
    }

    private Vector2 GetRandomPosOnScreen()
    {
        return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
    }

    private void GotItem(Shoot_item_prefab obj)
    {
        itemType type = obj.type;
        audioManager.PlaySFXbyTag(SfxTag.gotItem);
        switch (type)
        {
            case itemType.weapon:
                bullet_Manager.UpgradeBullet();
                gameManager.itemInfo_atk.Init(-1, bullet_Manager.currentBullet);
                break;
            case itemType.shield:
                gameManager.GetShield();
                gameManager.itemInfo_shield.Init(-1, 0);
                break;
            case itemType.bounce:
                bullet_Manager.bounceCount += 1;
                gameManager.itemInfo_bounce.Init(-1, bullet_Manager.bounceCount);
                if (bullet_Manager.bounceCount > 3) bullet_Manager.bounceCount = 3;
                break;
            case itemType.blackHole:
                FXManager.Instance.CreateFX(FXType.blackhole, obj.transform);
                audioManager.PlaySFXbyTag(SfxTag.blackhole);
                break;
            case itemType.spin:
                GameObject fx = FXManager.Instance.CreateFX(FXType.spin, player.transform);
                audioManager.PlaySFXbyTag(SfxTag.spin);
                fx.transform.SetParent(player.transform, true);
                gameManager.SetSpinMode(7f);
                gameManager.itemInfo_spin.Init(7);
                break;
        }
    }
    public void KillAll()
    {
        gameManager.itemInfo_atk.Hide();
        gameManager.itemInfo_shield.Hide();
        gameManager.itemInfo_bounce.Hide();
        gameManager.itemInfo_spin.Hide();
        
        for (int i = items.Count - 1; i >= 0; i--)
        {
            Destroy(items[i].transform.gameObject);
            items.RemoveAt(i);
        }

        totalItemCount = 0;
    }


}
    public enum itemType { weapon, bounce, shield, blackHole, spin, }
