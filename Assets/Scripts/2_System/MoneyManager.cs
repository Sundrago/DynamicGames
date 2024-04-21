using System;
using System.Collections.Generic;
using DG.Tweening;
using DynamicGames.Gachapon;
using DynamicGames.UI;
using DynamicGames.Utility;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace DynamicGames.System
{
    /// <summary>
    /// Manages the player's money, including tickets, gacha coins, and keys.
    /// </summary>
    public class MoneyManager : MonoBehaviour
    {
        public enum RewardType
        {
            Ticket,
            GachaCoin,
            Key
        }

        [Header("Managers and Controllers")] 
        [SerializeField] private GachaponManager gachaponManager;
        
        [Header("UI Elements")] 
        [SerializeField] private GameObject ticketHolder_ui;
        [SerializeField] private TextMeshProUGUI ticketCount_ui;
        [SerializeField] public GameObject gachaCoinHolder_ui;
        [SerializeField] private TextMeshProUGUI gachaCoinCount_ui;
        [SerializeField] public GameObject keyHolder_ui;
        [SerializeField] private TextMeshProUGUI keyCount_ui;
        [SerializeField] private Transform coin2DHolder;
        [SerializeField] private TextMeshProUGUI coin2DAmountText;
        [SerializeField] private GameObject ticket_prefab, gachaCoin_prefab, key_prefab;

        [Header("ObjectPools")] 
        public List<ObjectPool<GameObject>> obj_pools = new();

        private int ticketCount, gachaCoinCount, keyCount;
        public static MoneyManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetUpUI();
            InitializeRewards();
        }

        private void SetUpUI()
        {
            coin2DAmountText.gameObject.SetActive(false);
            ticketCount = PlayerData.GetInt(DataKey.ticketCount);
            gachaCoinCount = PlayerData.GetInt(DataKey.gachaCoinCount);
            keyCount = PlayerData.GetInt(DataKey.keyCount);
            UpdateUI();
        }

        private void InitializeRewards()
        {
            foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
            {
                var new_pool = new ObjectPool<GameObject>(() =>
                    {
                        GameObject obj;
                        switch (type)
                        {
                            case RewardType.Ticket:
                                obj = Instantiate(ticket_prefab, coin2DHolder);
                                break;
                            case RewardType.GachaCoin:
                                obj = Instantiate(gachaCoin_prefab, coin2DHolder);
                                break;
                            case RewardType.Key:
                                obj = Instantiate(key_prefab, coin2DHolder);
                                break;
                            default:
                                obj = Instantiate(ticket_prefab, coin2DHolder);
                                break;
                        }

                        return obj;
                    }, obj => { obj.gameObject.SetActive(true); }, obj => { obj.gameObject.SetActive(false); },
                    obj => { Destroy(obj.gameObject); }, true, 12, 15);

                obj_pools.Add(new_pool);
            }
        }

        public int GetCount(RewardType type)
        {
            switch (type)
            {
                case RewardType.Ticket:
                    return ticketCount;
                case RewardType.GachaCoin:
                    return gachaCoinCount;
                case RewardType.Key:
                    return keyCount;
            }

            return -1;
        }

        [Button]
        public void AddTicket(RewardType rewardType, int amount)
        {
            switch (rewardType)
            {
                case RewardType.Ticket:
                    ProcessAddReward(ref ticketCount, ticketCount_ui, amount);
                    break;

                case RewardType.GachaCoin:
                    ProcessAddReward(ref gachaCoinCount, gachaCoinCount_ui, amount);
                    break;

                case RewardType.Key:
                    ProcessAddReward(ref keyCount, keyCount_ui, amount);
                    UnlockBtnManager.Instance.SetBtnActive();
                    break;
            }

            UpdateUI();
            gachaponManager.SetBtnActive();
        }

        public void ProcessAddReward(ref int count, TextMeshProUGUI uiText, int amount)
        {
            var startValue = count;
            count += amount;
            var endValue = startValue + amount;
            DOVirtual.Int(startValue, endValue, 0.5f,
                value => { uiText.text = Mathf.Round(value).ToString(); });
        }

        public bool HasEnoughTicket(RewardType _type, int amount)
        {
            switch (_type)
            {
                case RewardType.Ticket:
                    return ticketCount >= amount;
                case RewardType.GachaCoin:
                    return gachaCoinCount >= amount;
                case RewardType.Key:
                    return keyCount >= amount;
                default:
                    return false;
            }
        }

        [Button]
        public bool SubtractMoney(RewardType rewardType, int amount)
        {
            if (!HasEnoughTicket(rewardType, amount)) return false;
            
            switch (rewardType)
            {
                case RewardType.Ticket:
                    ProcessSubtractReward(ref ticketCount, ticketCount_ui, amount);
                    break;
                case RewardType.GachaCoin:
                    ProcessSubtractReward(ref gachaCoinCount, gachaCoinCount_ui, amount);
                    break;
                case RewardType.Key:
                    ProcessSubtractReward(ref keyCount, keyCount_ui, amount);
                    break;
                default:
                    break;
            }

            gachaponManager.SetBtnActive();
            UpdateUI();
            return true;
        }

        private void ProcessSubtractReward(ref int count, TextMeshProUGUI uiText, int amount)
        {
            var startValue = count;
            count -= amount;

            var endValue = startValue - amount;

            DOVirtual.Int(startValue, endValue, 0.5f,
                value => { uiText.text = Mathf.Round(value).ToString(); }
            );
        }

        [Button]
        public void Reward2DAnimation(RewardType rewardType, Vector3 startPos, int count, float _velocity = 0.5f,
            float startAngle = 0f, float endAngle = 2f)
        {
            for (var i = 0; i < count; i++)
            {
                var durationFactor = GetDurationFactor(rewardType);
                var obj = obj_pools[(int)rewardType].Get();
                var path = GetAnimationPath(startPos, rewardType, _velocity, obj, startAngle, endAngle);
                SetupRewardAnimation(obj, path, durationFactor, rewardType);
            }

            if (rewardType == RewardType.Ticket) ShowTicketCountUI(count, startPos);
        }

        private float GetDurationFactor(RewardType type)
        {
            var durationFactor = Random.Range(1f, 2f);
            if (type == RewardType.Key) durationFactor *= 1.5f;
            return durationFactor;
        }

        private Vector3[] GetAnimationPath(Vector3 startPos, RewardType rewardType, float _velocity, GameObject obj,
            float startAngle, float endAngle)
        {
            SetupInitialTransform(obj, startPos, rewardType);

            var path = new Vector3[3];
            var velocity = _velocity * Random.Range(0.8f, 1.2f);
            var angle = Random.Range(startAngle, endAngle) * Mathf.PI;
            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);
            path[2] = GetPath2(rewardType);
            var diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - diff * Random.Range(0.3f, 0.8f);

            return path;
        }

        private void SetupInitialTransform(GameObject obj, Vector3 startPos, RewardType rewardType)
        {
            obj.transform.localScale = Vector3.one;
            obj.transform.position = startPos;
            obj.transform.eulerAngles =
                new Vector3(0, 0, rewardType == RewardType.GachaCoin ? 0 : Random.Range(0, 360));
        }

        private Vector3 GetPath2(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.Ticket:
                    return ticketHolder_ui.transform.position;
                case RewardType.GachaCoin:
                    return gachaCoinHolder_ui.transform.position;
                case RewardType.Key:
                    return keyHolder_ui.transform.position;
            }

            return Vector3.zero;
        }

        private void SetupRewardAnimation(GameObject obj, Vector3[] path, float durationFactor, RewardType rewardType)
        {
            obj.transform.DORotate(Vector3.zero, durationFactor).SetEase(Ease.InOutExpo);
            obj.transform.DOMove(path[0], 0.3f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    obj.transform.DOPath(path, 0.7f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase(Ease.InOutCubic)
                        .OnComplete(() => { obj_pools[(int)rewardType].Release(obj); });
                    obj.transform.DOScale(Vector3.zero, 0.7f * durationFactor)
                        .SetEase(Ease.InQuart)
                        .OnComplete(() => { OnAnimationComplete(rewardType); });
                });
        }

        private void OnAnimationComplete(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.Ticket:
                    DoPunchAnimationAndAddTicket(ticketHolder_ui, RewardType.Ticket, SfxTag.EarnedTicket);
                    break;
                case RewardType.GachaCoin:
                    DoPunchAnimationAndAddTicket(gachaCoinHolder_ui, RewardType.GachaCoin, SfxTag.EarnedCoin);
                    break;
                case RewardType.Key:
                    DoPunchAnimationAndAddTicket(keyHolder_ui, RewardType.Key, SfxTag.EarnedKey);
                    break;
            }
        }

        private void DoPunchAnimationAndAddTicket(GameObject uiElement, RewardType rewardType, SfxTag sfxTag)
        {
            DOTween.Kill(uiElement.transform);
            uiElement.transform.localScale = Vector3.one;
            uiElement.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f)
                .OnComplete(() => { uiElement.transform.localScale = Vector3.one; });
            AddTicket(rewardType, 1);
            AudioManager.Instance.PlaySfxByTag(sfxTag);
        }

        private void ShowTicketCountUI(int count, Vector3 startPos)
        {
            coin2DAmountText.text = "+" + count;
            coin2DAmountText.color = Color.white;
            coin2DAmountText.transform.localScale = Vector3.one;

            DOTween.Kill(coin2DAmountText.transform);
            coin2DAmountText.transform.DOPunchScale(Vector3.one * 0.4f, 1f);
            coin2DAmountText.DOFade(0, 1.5f)
                .SetDelay(0.8f)
                .OnComplete(() => { coin2DAmountText.gameObject.SetActive(false); });

            coin2DAmountText.transform.position = startPos;
            coin2DAmountText.gameObject.SetActive(true);
        }

        public void ShowPanel()
        {
            gameObject.transform.DOLocalMoveY(0, 2f)
                .SetEase(Ease.OutExpo);
            UpdateUI();
        }

        public void HidePanel()
        {
            gameObject.transform.DOLocalMoveY(1000, 2f)
                .SetEase(Ease.OutExpo);
        }

        private void UpdateUI()
        {
            ticketCount_ui.text = ticketCount.ToString();
            gachaCoinCount_ui.text = gachaCoinCount.ToString();
            keyCount_ui.text = keyCount.ToString();

            PlayerData.SetInt(DataKey.ticketCount, ticketCount);
            PlayerData.SetInt(DataKey.gachaCoinCount, gachaCoinCount);
            PlayerData.SetInt(DataKey.keyCount, keyCount);
        }
    }
}