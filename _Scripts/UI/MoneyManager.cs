using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    
    [SerializeField] private GameObject ticket_prefab, gachaCoin_prefab, key_prefab;
    
    [SerializeField]
    private GameObject ticketHolder_ui;
    [SerializeField]
    private TextMeshProUGUI ticketCount_ui;


    [SerializeField]
    public  GameObject gachaCoinHolder_ui;
    [SerializeField]
    private TextMeshProUGUI gachaCoinCount_ui;

    
    [SerializeField]
    public  GameObject keyHolder_ui;
    [SerializeField]
    private TextMeshProUGUI keyCount_ui;
    
    private int ticketCount, gachaCoinCount, keyCount;

    public List<ObjectPool<GameObject>> obj_pools = new List<ObjectPool<GameObject>>();


    [SerializeField]
    private GachaponManager gachaponManager;


    public enum RewardType { Ticket, GachaCoin, Key }

    public int GetCount(RewardType type)
    {
        switch (type)
        {
            case RewardType.Ticket:
                return ticketCount;
                break;
            case RewardType.GachaCoin :
                return gachaCoinCount;
                break;
            case RewardType.Key:
                return keyCount;
                break;
        }

        return -1;
    }
    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ticketCount = PlayerPrefs.GetInt("ticketCount", 0);
        gachaCoinCount = PlayerPrefs.GetInt("gachaCoinCount", 0);
        keyCount = PlayerPrefs.GetInt("keyCount", 0);
        UpdateUI();
        
        foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
        {
            int defaultCapacity = 5;
            int maxCapacity = 30;

            ObjectPool<GameObject> new_pool = new ObjectPool<GameObject>(() => {
                GameObject obj;
                switch (type)
                {
                    case RewardType.Ticket:
                        obj = Instantiate(ticket_prefab, gameObject.transform);
                        break;
                    case RewardType.GachaCoin:
                        obj = Instantiate(gachaCoin_prefab, gameObject.transform);
                        break;
                    case RewardType.Key:
                        obj = Instantiate(key_prefab, gameObject.transform);
                        break;
                    default: //CoinType.Oil:
                        obj = Instantiate(ticket_prefab, gameObject.transform);
                        break;
                }
                return obj;
            }, obj => {
                obj.gameObject.SetActive(true);
            }, obj => {
                obj.gameObject.SetActive(false);
            }, obj => {
                Destroy(obj.gameObject);
            }, true, 12, 15);
            
            obj_pools.Add(new_pool);
        }
    }

    public void AddTicket(RewardType _type, int amount)
    {
        // AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.coinInJar);
        int startValue, endValue;
        switch (_type)
        {
            case RewardType.Ticket:
                startValue = ticketCount;
                ticketCount += amount;
                PlayerPrefs.SetInt("totalTicketCount", ticketCount);
                // ticketHolder_ui.transform.localScale = Vector3.one;
                // ticketHolder_ui.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                
                endValue = startValue + amount;
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    ticketCount_ui.text = Mathf.Round(value).ToString();
                });
                break;
            case RewardType.GachaCoin:
                startValue = gachaCoinCount;
                gachaCoinCount += amount;
                PlayerPrefs.SetInt("gachaCoinCount", gachaCoinCount);
                // gachaCoinHolder_ui.transform.localScale = Vector3.one;
                // gachaCoinHolder_ui.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                
                endValue = startValue + amount;
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    gachaCoinCount_ui.text = Mathf.Round(value).ToString();
                });
                break;
            case RewardType.Key:
                startValue = keyCount;
                keyCount += amount;
                PlayerPrefs.SetInt("keyCount", keyCount);
                // keyHolder_ui.transform.localScale = Vector3.one;
                // keyHolder_ui.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f);
                
                UnlockBtnManager.Instance.SetBtnActive();
                endValue = startValue + amount;
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    keyCount_ui.text = Mathf.Round(value).ToString();
                });
                break;
            default:
                break;
        }
        UpdateUI();
        gachaponManager.SetBtnActive();
    }

    public bool HasEnoughTicket(RewardType _type, int amount)
    {
        switch (_type)
        {
            case RewardType.Ticket:
                return (ticketCount >= amount);
                break;
            case RewardType.GachaCoin:
                return (gachaCoinCount >= amount);
                break;
            case RewardType.Key:
                return (keyCount >= amount);
                break;
            default:
                return false;
                break;
        }
    }

    public bool SubtractTicket(RewardType _type, int amount)
    {
        if (!HasEnoughTicket(_type, amount)) return false;
        
        int startValue, endValue;
        switch (_type)
        {
            case RewardType.Ticket:
                startValue = ticketCount;
                ticketCount -= amount;
                PlayerPrefs.SetInt("totalTicketCount", ticketCount);
                endValue = startValue - amount;
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    ticketCount_ui.text = Mathf.Round(value).ToString();
                });
                
                break;
            case RewardType.GachaCoin:
                startValue = gachaCoinCount;
                gachaCoinCount -= amount;
                PlayerPrefs.SetInt("gachaCoinCount", gachaCoinCount);
                endValue = startValue - amount;
                
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    gachaCoinCount_ui.text = Mathf.Round(value).ToString();
                });
                break;
            case RewardType.Key:
                startValue = keyCount;
                keyCount -= amount;
                PlayerPrefs.SetInt("keyCount", keyCount);
                endValue = startValue - amount;
                
                DOVirtual.Int(startValue, endValue, 0.5f, value => {
                    keyCount_ui.text = Mathf.Round(value).ToString();
                });
                break;
            default:
                startValue = 0;
                break;
        }
        gachaponManager.SetBtnActive();
        UpdateUI();
        return true;
    }
    
    [Button]
    public void Coin2DAnim(RewardType type, Vector3 startPos, int count, float _velocity = 0.5f,  float startAngle = 0f, float endAngle = 2f)
    {
        for (int i = 0; i < count; i++)
        {
            float durationFactor = Random.Range(1f, 2f);
            if (type == RewardType.Key) durationFactor *= 1.5f;

            Vector3[] path = new Vector3[3];
            path[0] = startPos;

            GameObject obj = obj_pools[(int)type].Get();
            float velocity = _velocity * Random.Range(0.8f, 1.2f);

            switch (type)
            {
                case RewardType.Ticket:
                    path[2] = ticketHolder_ui.transform.position;
                    break;
                case RewardType.GachaCoin:
                    path[2] = gachaCoinHolder_ui.transform.position;
                    break;
                case RewardType.Key:
                    path[2] = keyHolder_ui.transform.position;
                    break;
                default:
                    break;
            }

            obj.transform.localScale = Vector3.one;
            obj.transform.position = startPos;
            obj.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));

            float angle = Random.Range(startAngle, endAngle) * Mathf.PI;
            path[0] = startPos + new Vector3(Mathf.Sin(angle) * velocity, Mathf.Cos(angle) * velocity, 0);

            Vector3 diff = startPos - path[0];
            path[1] = Vector3.Lerp(path[0], path[2], Random.Range(0.3f, 0.5f)) - (diff * Random.Range(0.3f, 0.8f));

            obj.transform.DORotate(Vector3.zero, durationFactor).SetEase(Ease.InOutExpo);
            obj.transform.DOMove(path[0], 0.3f * durationFactor)
                .SetEase(Ease.OutCirc)
                .OnComplete(() => {
                    obj.transform.DOPath(path, 0.7f * durationFactor, PathType.CatmullRom, PathMode.TopDown2D, 1)
                        .SetEase((Ease.InOutCubic))
                        .OnComplete(() => {
                            obj_pools[(int)type].Release(obj);
                        });
                    obj.transform.DOScale(Vector3.zero, 0.7f * durationFactor)
                        .SetEase(Ease.InQuart)
                        .OnComplete(() => {
                            switch (type)
                            {
                                case RewardType.Ticket:
                                    DOTween.Kill(ticketHolder_ui.transform);
                                    ticketHolder_ui.transform.localScale = Vector3.one;
                                    ticketHolder_ui.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f)
                                        .OnComplete(() =>
                                        {
                                            ticketHolder_ui.transform.localScale = Vector3.one;
                                        });
                                    AddTicket(RewardType.Ticket, 1);
                                    AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.earnTicket);
                                    break;
                                case RewardType.GachaCoin:
                                    gachaCoinHolder_ui.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f)
                                        .OnComplete(() =>
                                        {
                                            gachaCoinHolder_ui.transform.localScale = Vector3.one;
                                        });
                                    AddTicket(RewardType.GachaCoin, 1);
                                    AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.earnCoin);
                                    break;
                                case RewardType.Key:
                                    keyHolder_ui.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f)
                                        .OnComplete(() =>
                                        {
                                            keyHolder_ui.transform.localScale = Vector3.one;
                                        });
                                    AddTicket(RewardType.Key, 1);
                                    AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.earnKey);
                                    break;
                                default:
                                    break;
                            }
                        });
                });
        }
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
        
        PlayerPrefs.SetInt("ticketCount", ticketCount);
        PlayerPrefs.SetInt("gachaCoinCount", gachaCoinCount);
        PlayerPrefs.SetInt("keyCount", keyCount);
        PlayerPrefs.Save();
    }
    
}
