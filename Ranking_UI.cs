using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;



public class Ranking_UI : MonoBehaviour
{
    [SerializeField]
    private RankingManager rankingManager;
    [SerializeField]
    private LeaderboardManger leaderboardManager;
    [SerializeField]
    private Slider slider_ui;
    [SerializeField]
    private TextMeshProUGUI score_text_ui;
    [SerializeField]
    private Image tier_image_ui;
    [SerializeField]
    private RectTransform tier_icon_group;
    [SerializeField]
    private RectTransform[] tierIconRectTransforms;
    [SerializeField]
    private float tierIconGroupOffsetY;

    private GameType gameType;
    
    public void OnSliderValueChanged()
    {
        // float percent = Mathf.Round(slider_ui.value * 1000f) / 10f;
        // print(percent);
        // Tiers tier = rankingManager.GetTiersFromRankInPercent(percent);
        // Sprite tier_sprite = rankingManager.rankSprites[tier];
        // tier_icon_group.anchoredPosition = new Vector2(0, 2975 * slider_ui.value);
        //
        // //if (tier_image_ui.sprite != tier_sprite) tier_image_ui.sprite = tier_sprite;
        // score_text_ui.text = percent + "%";
    }

    [Button]
    private void SetTierIconAnim(float startPercent, float percent, float duration = 2f)
    {
        startPercent = 0;
        Tiers tier = rankingManager.GetTiersFromRankInPercent(percent);
        
        slider_ui.DOValue((startPercent + percent)/200f, duration/2f)
            .SetEase(Ease.InExpo)
            .OnComplete(() => {
                slider_ui.DOValue(percent / 100f, duration / 2f)
                    .SetEase(Ease.OutElastic);
            });

        Tiers startTier = rankingManager.GetTiersFromRankInPercent(startPercent);
        float startPosY = tierIconRectTransforms[(int)startTier].anchoredPosition.y * -1f + tierIconGroupOffsetY;
        float targetPosY = tierIconRectTransforms[(int)tier].anchoredPosition.y * -1f + tierIconGroupOffsetY;
        float targetMidPosY = (targetPosY + tier_icon_group.anchoredPosition.y) / 2f;
        tier_icon_group.transform.localPosition = new Vector3(0, startPosY, 0);
        tier_icon_group.DOLocalMove(new Vector3(0, targetMidPosY, 0), duration / 2f)
            .SetEase(Ease.InExpo)
            .OnComplete(() => {
                tier_icon_group.DOLocalMove(new Vector3(0, targetPosY, 0), duration / 2f)
                    .SetEase(Ease.OutElastic);
            });
    }

    public void OpenLeaderboard()
    {
        leaderboardManager.OpenLeaderboardAt(gameType);
    }
    
    public void Show(GameType tier)
    {
        
    }

    public void Hide()
    {
        
    }
}
