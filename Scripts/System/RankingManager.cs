using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class RankingManager : SerializedMonoBehaviour
{
    [SerializeField]
    private LeaderboardManger leaderboardManager;
    [SerializeField]
    public Dictionary<Tiers, Sprite> rankSprites;
    [SerializeField]
    private Dictionary<GameType, GameObject> iconBadges;
    [SerializeField]
    private Ranking_UI ranking_UI;

    public Tiers GetTiersFromRank(float percent)
    {
        if (percent < 15) return Tiers.bronze0;
        else if (percent < 30) return Tiers.bronze1;
        else if (percent < 40) return Tiers.bronze2;
        else if (percent < 50) return Tiers.silver0;
        else if (percent < 60) return Tiers.silver1;
        else if (percent < 70) return Tiers.silver2;
        else if (percent < 78) return Tiers.gold0;
        else if (percent < 84) return Tiers.gold1;
        else if (percent < 90) return Tiers.gold2;
        else if (percent < 95) return Tiers.diamond0;
        else if (percent < 98) return Tiers.diamond1;
        else return Tiers.diamond2;
    }
    public IEnumerator UpdatetRanks()
    {
        foreach (KeyValuePair<GameType, GameObject> iconBadge in iconBadges)
        {
            GameType gameType = iconBadge.Key;
            GameObject badge = iconBadge.Value;
            
            Tiers tier = GetTierByGameType(gameType);
            
            if (tier == Tiers.undefined)
            {
                badge.SetActive(false);
            }
            else
            {
                int displayed = PlayerPrefs.GetInt("previousTier_" + gameType);
                Image badgeSpriteRenderer = badge.GetComponent<Image>();
                badgeSpriteRenderer.sprite = rankSprites[tier];
                if (!badge.activeSelf)
                {
                    badgeSpriteRenderer.color = Color.white;
                    badgeSpriteRenderer.DOFade(0f, 0.3f)
                        .From();
                }
                badge.SetActive(true);
                
                if ((int)tier > displayed) FXManager.Instance.CreateFX(FXType.tier_up, badge.transform);
                else if((int)tier < displayed) FXManager.Instance.CreateFX(FXType.tier_down, badge.transform);
                else continue;
                
                PlayerPrefs.SetInt("previousTier_" + gameType, (int)tier);
            }
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.5f));
        }
    }

    public Tiers GetTierByGameType(GameType gameType)
    {
        if (!PlayerPrefs.HasKey("rank_" + gameType))
        {
            PlayerPrefs.SetInt("rank_" + gameType, -1);
        }
        int rank = PlayerPrefs.GetInt("rank_" + gameType);
        int totalPlayerCount = GetTotalPlayerCountByGameType(gameType);
        print("rank :" + rank);
        print("totalCount :" + totalPlayerCount);
        
        Tiers tier;
        if (totalPlayerCount == 0 || rank == -1)
            tier = Tiers.undefined;
        else tier = GetTiersFromRank(GetPercent(rank, totalPlayerCount));

        return tier;
    }

    public int GetTotalPlayerCountByGameType(GameType gameType)
    {
        int totalCount = 0;
        int.TryParse(PlayerPrefs.GetString("userCount_" + gameType), out totalCount);
        return totalCount;
    }

    public float GetPercent(int rank, int totalCount)
    {
        float percent = (1 - (float)rank / (float)totalCount) * 100f;
        return Mathf.Clamp(percent, 0f, 100f);
    }

    public float GetRankPercentByGameType(GameType gameType)
    {
        int rank = PlayerPrefs.GetInt("rank_" + gameType);
        int totalCount = 0;
        int.TryParse(PlayerPrefs.GetString("userCount_" + gameType), out totalCount);
        float rankingPercent = (1 - (float)rank / (float)totalCount) * 100f;
        return Mathf.Round((rankingPercent * 10f) / 1f);
    }
}

[Serializable]
public enum Tiers { bronze0, bronze1, bronze2, silver0, silver1, silver2, gold0, gold1, gold2, diamond0, diamond1, diamond2, undefined }