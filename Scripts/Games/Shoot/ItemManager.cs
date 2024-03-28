using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Shoot
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] private Boundaries boundaries, islandBounaries;

        [FormerlySerializedAs("itemPrefab")] [SerializeField]
        private ItemController itemController;

        [SerializeField] private Transform player;
        [SerializeField] private BulletManager bullet_Manager;

        [SerializeField] private Sprite[] item_imgs;
        [SerializeField] private GameManager gameManager;

        [FormerlySerializedAs("audioCtrl")] [SerializeField]
        private AudioManager audioManager;

        public List<ItemController> items = new();

        private Vector2 screenBounds;
        private int totalItemCount;

        private void Start()
        {
            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
            KillAll();
        }

        private void Update()
        {
            if (gameManager.state != GameManager.ShootGameState.playing) return;

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var obj = items[i];

                //Update Item if Timer ends
                if (obj.GetNormalizedTimer() < 0.01f) UpdateItem(obj);

                //Move and bounce on wall
                if (obj == null) continue;

                obj.transform.Translate(obj.Velocity * Time.deltaTime * obj.RandomVector);
                if (obj.transform.position.x < boundaries.left.position.x)
                {
                    obj.transform.position = new Vector3(boundaries.left.position.x, obj.transform.position.y, 0f);
                    obj.RandomVector = new Vector3(obj.RandomVector.x * -1f, obj.RandomVector.y, 0f);
                }
                else if (obj.transform.position.x > boundaries.right.position.x)
                {
                    obj.transform.position = new Vector3(boundaries.right.position.x, obj.transform.position.y, 0f);
                    obj.RandomVector = new Vector3(obj.RandomVector.x * -1f, obj.RandomVector.y, 0f);
                }

                if (obj.transform.position.y > boundaries.top.position.y)
                {
                    obj.transform.position = new Vector3(obj.transform.position.x, boundaries.top.position.y, 0f);
                    obj.RandomVector = new Vector3(obj.RandomVector.x, obj.RandomVector.y * -1f, 0f);
                }
                else if (obj.transform.position.y < boundaries.btm.position.y)
                {
                    obj.transform.position = new Vector3(obj.transform.position.x, boundaries.btm.position.y, 0f);
                    obj.RandomVector = new Vector3(obj.RandomVector.x, obj.RandomVector.y * -1f, 0f);
                }

                //Move Out When On Island
                if (obj.transform.position.x > islandBounaries.left.position.x &&
                    obj.transform.position.x < islandBounaries.right.position.x &&
                    obj.transform.position.y > islandBounaries.btm.position.y &&
                    obj.transform.position.y < islandBounaries.top.position.y)
                    if (obj.RandomVector.y > -1.5f)
                        obj.RandomVector.y += -15f * Time.deltaTime;

                //When Player Get near item
                if (Vector2.Distance(player.position, obj.transform.position) < 0.3)
                {
                    items.Remove(obj);
                    obj.transform.DOScale(0f, 0.25f)
                        .SetEase(Ease.InOutElastic)
                        .OnComplete(() => { Destroy(obj.transform.gameObject); });
                    GotItem(obj);
                    FXManager.Instance.CreateFX(FXType.ItemHit, obj.transform);
                }
            }
        }

        public void SpawnItem()
        {
            if (gameManager.state != GameManager.ShootGameState.playing) return;
            if (items.Count >= 3) return;
            // if(gameManager.stage * 2.5f < totalItemCount) return;

            var new_item = Instantiate(itemController, gameObject.transform);
            var rnd = Random.Range(0, 5);
            var type = (ItemType)rnd;

            if (type == ItemType.Shield && gameManager.shield != null)
            {
                type = GetRandomSingleItem();
            }
            else if (type == ItemType.Bounce)
            {
                var chance = bullet_Manager.bounceCount * 0.25f;
                if (Random.Range(0f, 1f) < chance) type = GetRandomSingleItem();
            }

            new_item.Init(GetRandomPosOnScreen(), type, item_imgs[(int)type]);
            new_item.transform.DOScale(new Vector3(0, 0, 0), 1f)
                .From();
            new_item.gameObject.SetActive(true);
            items.Add(new_item);

            totalItemCount += 1;
        }

        private ItemType GetRandomSingleItem()
        {
            return (ItemType)Random.Range(2, 4);
        }

        public void UpdateItem(ItemController item)
        {
            if (gameManager.state != GameManager.ShootGameState.playing) return;
            var rnd = Random.Range(0, 5);

            var type = (ItemType)rnd;
            if (type == ItemType.Shield && gameManager.shield != null)
            {
                type = GetRandomSingleItem();
            }
            else if (type == ItemType.Bounce)
            {
                var chance = bullet_Manager.bounceCount * 0.25f;
                if (Random.Range(0f, 1f) < chance) type = GetRandomSingleItem();
            }

            item.Init(item.transform.localPosition, (ItemType)rnd, item_imgs[rnd]);
            item.transform.DOPunchScale(new Vector3(0.03f, 0.03f, 0), 0.5f).SetEase(Ease.InOutQuad);
        }

        private Vector2 GetRandomPosOnScreen()
        {
            return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
        }

        private void GotItem(ItemController obj)
        {
            var type = obj.itemType;
            audioManager.PlaySFXbyTag(SfxTag.gotItem);
            switch (type)
            {
                case ItemType.Weapon:
                    bullet_Manager.UpgradeBullet();
                    gameManager.itemInfo_atk.Init(-1, bullet_Manager.currentBulletObj);
                    break;
                case ItemType.Shield:
                    gameManager.GetShield();
                    gameManager.itemInfo_shield.Init(-1);
                    break;
                case ItemType.Bounce:
                    bullet_Manager.bounceCount += 1;
                    gameManager.itemInfo_bounce.Init(-1, bullet_Manager.bounceCount);
                    if (bullet_Manager.bounceCount > 3) bullet_Manager.bounceCount = 3;
                    break;
                case ItemType.BlackHole:
                    FXManager.Instance.CreateFX(FXType.blackhole, obj.transform);
                    audioManager.PlaySFXbyTag(SfxTag.blackhole);
                    break;
                case ItemType.Spin:
                    var fx = FXManager.Instance.CreateFX(FXType.spin, player.transform);
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

            for (var i = items.Count - 1; i >= 0; i--)
            {
                Destroy(items[i].transform.gameObject);
                items.RemoveAt(i);
            }

            totalItemCount = 0;
        }
    }

    public enum ItemType
    {
        Weapon,
        Bounce,
        Shield,
        BlackHole,
        Spin
    }
}