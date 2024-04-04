using System;
using System.Collections;
using System.Collections.Generic;
using Core.UI;
using DG.Tweening;
using Games;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core.System
{
    public class RankingManager : SerializedMonoBehaviour
    {
        [Header("Managers and Controllers")] [SerializeField]
        private LeaderboardManger leaderboardManager;

        [FormerlySerializedAs("ranking_UI")] [SerializeField] private LeaderboardUI leaderboardUI;

        private readonly SortedDictionary<int, Tiers> TiersDictionary = new()
        {
            { 15, Tiers.Bronze0 },
            { 30, Tiers.Bronze1 },
            { 40, Tiers.Bronze2 },
            { 50, Tiers.Silver0 },
            { 60, Tiers.Silver1 },
            { 70, Tiers.Silver2 },
            { 78, Tiers.Gold0 },
            { 84, Tiers.Gold1 },
            { 90, Tiers.Gold2 },
            { 95, Tiers.Diamond0 },
            { 98, Tiers.Diamond1 },
            { 100, Tiers.Diamond2 }
        };

        [SerializeField] private Dictionary<GameType, GameObject> iconBadges;

        [Header("UI Elements")] [SerializeField]
        public Dictionary<Tiers, Sprite> rankSprites;

        public Tiers GetTiersFromRank(float percent)
        {
            foreach (var tier in TiersDictionary)
                if (percent < tier.Key)
                    return tier.Value;

            return Tiers.Undefined;
        }

        public IEnumerator UpdateRanks()
        {
            foreach (var iconBadge in iconBadges)
            {
                var gameType = iconBadge.Key;
                var badge = iconBadge.Value;

                var tier = GetTierByGameType(gameType);

                if (tier == Tiers.Undefined)
                {
                    badge.SetActive(false);
                }
                else
                {
                    var displayed = PlayerPrefs.GetInt("previousTier_" + gameType);
                    var badgeSpriteRenderer = badge.GetComponent<Image>();
                    badgeSpriteRenderer.sprite = rankSprites[tier];
                    if (!badge.activeSelf)
                    {
                        badgeSpriteRenderer.color = Color.white;
                        badgeSpriteRenderer.DOFade(0f, 0.3f)
                            .From();
                    }

                    badge.SetActive(true);

                    if ((int)tier > displayed) FXManager.Instance.CreateFX(FXType.TierUp, badge.transform);
                    else if ((int)tier < displayed) FXManager.Instance.CreateFX(FXType.TierDown, badge.transform);
                    else continue;

                    PlayerPrefs.SetInt("previousTier_" + gameType, (int)tier);
                }

                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        public Tiers GetTierByGameType(GameType gameType)
        {
            if (!PlayerPrefs.HasKey("rank_" + gameType)) PlayerPrefs.SetInt("rank_" + gameType, -1);
            var rank = PlayerPrefs.GetInt("rank_" + gameType);
            var totalPlayerCount = GetTotalPlayerCountByGameType(gameType);
            print("rank :" + rank);
            print("totalCount :" + totalPlayerCount);

            Tiers tier;
            if (totalPlayerCount == 0 || rank == -1)
                tier = Tiers.Undefined;
            else tier = GetTiersFromRank(GetRankInPercent(rank, totalPlayerCount));

            return tier;
        }

        public int GetTotalPlayerCountByGameType(GameType gameType)
        {
            var totalCount = 0;
            int.TryParse(PlayerPrefs.GetString("userCount_" + gameType), out totalCount);
            return totalCount;
        }

        public float GetRankInPercent(int rank, int totalCount)
        {
            var percent = (1 - rank / (float)totalCount) * 100f;
            return Mathf.Clamp(percent, 0f, 100f);
        }

        public float GetRankInPercentByGameType(GameType gameType)
        {
            var rank = PlayerPrefs.GetInt("rank_" + gameType);
            var totalCount = 0;
            int.TryParse(PlayerPrefs.GetString("userCount_" + gameType), out totalCount);
            var rankingPercent = (1 - rank / (float)totalCount) * 100f;
            return Mathf.Round(rankingPercent * 10f / 1f);
        }
    }

    [Serializable]
    public enum Tiers
    {
        Bronze0,
        Bronze1,
        Bronze2,
        Silver0,
        Silver1,
        Silver2,
        Gold0,
        Gold1,
        Gold2,
        Diamond0,
        Diamond1,
        Diamond2,
        Undefined
    }
}