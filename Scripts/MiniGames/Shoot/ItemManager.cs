using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.System;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    public enum ItemType
    {
        Weapon,
        Bounce,
        Shield,
        BlackHole,
        Spin
    }

    /// <summary>
    ///     Controls the spawning and updating of items in a shooting mini-game.
    /// </summary>
    public class ItemManager : MonoBehaviour
    {

        [Header("Managers and Controllers")] 
        [SerializeField] private GameManager gameManager;
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private ItemController itemController;
        [SerializeField] private AudioManager audioManager;

        [Header("Game Components")] 
        [SerializeField] private Boundaries boundaries, islandBounaries;
        [SerializeField] private Transform player;
        [SerializeField] private Sprite[] item_imgs;
        
        private const int MaxBounceCount = 3;
        
        private Vector2 screenBounds;
        private int totalItemCount;
        public List<ItemController> items { get; private set; }

        private void Start()
        {
            items = new List<ItemController>();
            screenBounds =
                Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                    Camera.main.transform.position.z));
            KillAll();
        }

        private void Update()
        {
            if (!IsGamePlaying()) return;

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                if (item == null) continue;

                UpdateItem(item);
                HandleMovement(item);
                BounceOffBoundary(item);
                HandleItemNearIsland(item);
                HandleItemInteraction(item);
            }
        }

        private bool IsGamePlaying()
        {
            return gameManager.state == GameManager.ShootGameState.playing;
        }

        public void UpdateItem(ItemController item)
        {
            if (item.GetNormalizedTimer() >= 0.01f) return;
            if (gameManager.state != GameManager.ShootGameState.playing) return;

            var type = GetItemType();

            item.Init(item.transform.localPosition, type, item_imgs[(int)type]);
            item.transform.DOPunchScale(new Vector3(0.03f, 0.03f, 0), 0.5f).SetEase(Ease.InOutQuad);
        }

        private ItemType GetItemType()
        {
            var rnd = Random.Range(0, 5);
            var type = (ItemType)rnd;
            if (type == ItemType.Shield && gameManager.shield != null)
            {
                type = GetRandomSingleItem();
            }
            else if (type == ItemType.Bounce)
            {
                var chance = bulletManager.bounceCount * 0.25f;
                if (Random.Range(0f, 1f) < chance) type = GetRandomSingleItem();
            }

            return type;
        }

        private void HandleMovement(ItemController item)
        {
            item.transform.Translate(item.Velocity * Time.deltaTime * item.RandomVector);
        }

        private void BounceOffBoundary(ItemController item)
        {
            if (item.transform.position.x < boundaries.left.position.x)
            {
                item.transform.position = new Vector3(boundaries.left.position.x, item.transform.position.y, 0f);
                item.RandomVector = new Vector3(item.RandomVector.x * -1f, item.RandomVector.y, 0f);
            }
            else if (item.transform.position.x > boundaries.right.position.x)
            {
                item.transform.position = new Vector3(boundaries.right.position.x, item.transform.position.y, 0f);
                item.RandomVector = new Vector3(item.RandomVector.x * -1f, item.RandomVector.y, 0f);
            }

            if (item.transform.position.y > boundaries.top.position.y)
            {
                item.transform.position = new Vector3(item.transform.position.x, boundaries.top.position.y, 0f);
                item.RandomVector = new Vector3(item.RandomVector.x, item.RandomVector.y * -1f, 0f);
            }
            else if (item.transform.position.y < boundaries.btm.position.y)
            {
                item.transform.position = new Vector3(item.transform.position.x, boundaries.btm.position.y, 0f);
                item.RandomVector = new Vector3(item.RandomVector.x, item.RandomVector.y * -1f, 0f);
            }
        }

        private void HandleItemNearIsland(ItemController item)
        {
            if (item.transform.position.x > islandBounaries.left.position.x &&
                item.transform.position.x < islandBounaries.right.position.x &&
                item.transform.position.y > islandBounaries.btm.position.y &&
                item.transform.position.y < islandBounaries.top.position.y)
                if (item.RandomVector.y > -1.5f)
                    item.RandomVector.y += -15f * Time.deltaTime;
        }

        private void HandleItemInteraction(ItemController item)
        {
            if (Vector2.Distance(player.position, item.transform.position) < 0.3)
            {
                items.Remove(item);
                item.transform.DOScale(0f, 0.25f).SetEase(Ease.InOutElastic).OnComplete(() =>
                {
                    Destroy(item.transform.gameObject);
                });
                GotItem(item);
                FXManager.Instance.CreateFX(FXType.ItemHit, item.transform);
            }
        }

        public void SpawnItem()
        {
            if (gameManager.state != GameManager.ShootGameState.playing) return;
            if (items.Count >= 3) return;

            var new_item = Instantiate(itemController, gameObject.transform);
            var type = GetItemType();

            new_item.Init(GetRandomPosOnScreen(), type, item_imgs[(int)type]);
            new_item.transform.DOScale(new Vector3(0, 0, 0), 1f)
                .From();
            new_item.gameObject.SetActive(true);
            items.Add(new_item);

            totalItemCount += 1;
        }

        private Vector2 GetRandomPosOnScreen()
        {
            return new Vector2(Random.Range(-1f, 1f) * screenBounds.x, Random.Range(-1f, 1f) * screenBounds.y);
        }

        private void GotItem(ItemController obj)
        {
            var type = obj.itemType;
            audioManager.PlaySfxByTag(SfxTag.AcquiredItem);

            switch (type)
            {
                case ItemType.Weapon:
                    HandleWeaponPickup();
                    break;
                case ItemType.Shield:
                    HandleShieldPickup();
                    break;
                case ItemType.Bounce:
                    HandleBouncePickup();
                    break;
                case ItemType.BlackHole:
                    HandleBlackHolePickup(obj);
                    break;
                case ItemType.Spin:
                    HandleSpinPickup();
                    break;
            }
        }

        private void HandleWeaponPickup()
        {
            bulletManager.UpgradeBullet();
            gameManager.itemInformationUIAtk.Init(-1, bulletManager.currentBulletObj);
        }

        private void HandleShieldPickup()
        {
            gameManager.GetShield();
            gameManager.itemInformationUIShield.Init(-1);
        }

        private void HandleBouncePickup()
        {
            bulletManager.bounceCount += 1;
            gameManager.itemInformationUIBounce.Init(-1, bulletManager.bounceCount);
            bulletManager.bounceCount = Mathf.Min(bulletManager.bounceCount, MaxBounceCount);
        }

        private void HandleBlackHolePickup(ItemController obj)
        {
            FXManager.Instance.CreateFX(FXType.Blackhole, obj.transform);
            audioManager.PlaySfxByTag(SfxTag.Blackhole);
        }

        private void HandleSpinPickup()
        {
            var fx = FXManager.Instance.CreateFX(FXType.Spin, player.transform);
            audioManager.PlaySfxByTag(SfxTag.Spin);
            fx.transform.SetParent(player.transform, true);
            gameManager.SetSpinMode(7f);
            gameManager.itemInformationUISpin.Init(7);
        }

        private ItemType GetRandomSingleItem()
        {
            return (ItemType)Random.Range(2, 4);
        }

        public void KillAll()
        {
            gameManager.itemInformationUIAtk.HideUI();
            gameManager.itemInformationUIShield.HideUI();
            gameManager.itemInformationUIBounce.HideUI();
            gameManager.itemInformationUISpin.HideUI();

            for (var i = items.Count - 1; i >= 0; i--)
            {
                Destroy(items[i].transform.gameObject);
                items.RemoveAt(i);
            }

            totalItemCount = 0;
        }
    }
}