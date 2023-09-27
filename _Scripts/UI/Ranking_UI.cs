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
    private TextMeshProUGUI score_text_ui, title_ui;
    [SerializeField]
    private Image tier_image_ui;
    [SerializeField]
    private RectTransform tier_icon_group;
    [SerializeField]
    private RectTransform[] tierIconRectTransforms;
    [SerializeField]
    private float tierIconGroupOffsetY;
    [SerializeField]
    private Image BG;
    [SerializeField]
    private Button openLeaderboard_btn, close_btn;
    [SerializeField]
    private Transform tierBoard_ui;
    [SerializeField]
    private SFXCTRL sfx;
    [SerializeField]
    private GameObject score_ui, login_ui, failed_ui;
    
    [SerializeField] 

    private GameType gameType;
    private bool canSkip = false;
    
    [Button]
    public IEnumerator ShowRankingUI(GameType _gameType, bool forceSHow = false)
    {
        gameType = _gameType;
        int previousRank = PlayerPrefs.GetInt("previousRank" + gameType);
        int newRank = PlayerPrefs.GetInt("rank_" + gameType);

        if (newRank == -1) SetUI(RankUIPage.Login);
        else SetUI(RankUIPage.Score);
        
        if(!forceSHow && (DOTween.IsTweening(tierBoard_ui.gameObject.transform) || previousRank == newRank))
        {
            sfx.ChangeBGMVolume(1f, 3f);
            gameObject.SetActive(false);
            yield break;
        }
        gameObject.SetActive(true);
        //Show Panel
        title_ui.text = leaderboardManager.GetLeaderboardByGameType(gameType).title;
        canSkip = false;
        close_btn.transform.localScale = Vector3.zero;
        openLeaderboard_btn.gameObject.transform.localScale = Vector3.zero;
        
        BG.color = new Color(0, 0, 0, 0);
        BG.DOFade(0.7f, 0.4f);
        
        tierBoard_ui.gameObject.transform.position = Vector3.zero;
        tierBoard_ui.gameObject.transform.DOLocalMoveY(2000, 0.5f)
            .From()
            .SetEase(Ease.OutBack);

        

        if (DOTween.IsTweening(slider_ui)) DOTween.Kill(slider_ui);
        if (DOTween.IsTweening(tier_icon_group.transform)) DOTween.Kill(tier_icon_group.transform);
        
        //Rank Anim
        int totalPlayerCount = rankingManager.GetTotalPlayerCountByGameType(gameType);
        if (previousRank > totalPlayerCount) totalPlayerCount = previousRank;
        if (newRank > totalPlayerCount) totalPlayerCount = newRank;
        
        print("totalPlayerCount : " + totalPlayerCount);
        float previousRankInPercent = rankingManager.GetPercent(previousRank, totalPlayerCount);
        float newRankInPercent = rankingManager.GetPercent(newRank, totalPlayerCount);
        Tiers previousTier = rankingManager.GetTiersFromRank(previousRankInPercent);
        Tiers newTier = rankingManager.GetTiersFromRank(newRankInPercent);

        float startPosY = tierIconRectTransforms[(int)previousTier].anchoredPosition.y * -1f + tierIconGroupOffsetY;
        float targetPosY = tierIconRectTransforms[(int)newTier].anchoredPosition.y * -1f + tierIconGroupOffsetY;
        float targetMidPosY = (targetPosY + tier_icon_group.anchoredPosition.y) / 2f;
        
        tier_icon_group.transform.localPosition = new Vector3(0, startPosY, 0);
        slider_ui.value = previousRankInPercent / 100f;
        PlayerPrefs.SetInt("previousRank" + gameType, newRank);
        
        yield return new WaitForSeconds(0.4f);
        
        float duration;
        if (previousTier < newTier)
        {
            duration = 4f;
            AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.rank_goup);

            slider_ui.DOValue((previousRankInPercent + newRankInPercent) / 200f, duration /5f * 4f)
                .SetEase(Ease.InOutQuart)
                .OnUpdate(() => {
                    float percent = Mathf.Round(slider_ui.value * 1000f) / 10f;
                    score_text_ui.text = percent + "%";
                })
                .OnComplete(() => {
                    sfx.ChangeBGMVolume(1f, 3f);
                });
        }
        else
        {
            close_btn.transform.DOScale(Vector3.one, 0.5f);
            openLeaderboard_btn.transform.DOScale(Vector3.one, 0.3f);
            canSkip = true;
            duration = 1f;
            
            slider_ui.DOValue((previousRankInPercent + newRankInPercent)/200f, duration/3f)
                .SetEase(Ease.InExpo)
                .OnUpdate(() => {
                    float percent = Mathf.Round(slider_ui.value * 1000f) / 10f;
                    score_text_ui.text = percent + "%";
                })
                .OnComplete(() => {
                    slider_ui.DOValue(newRankInPercent / 100f, duration / 3f)
                        .SetEase(Ease.OutElastic).OnUpdate(() => {
                            float percent = Mathf.Round(slider_ui.value * 1000f) / 10f;
                            score_text_ui.text = percent + "%";
                        });
                });
        }
        
        tier_icon_group.DOLocalMove(new Vector3(0, targetMidPosY, 0), duration / 3f)
            .SetEase(Ease.InExpo)
            .OnComplete(() => {
                tier_icon_group.DOLocalMove(new Vector3(0, targetPosY, 0), duration / 3f * 2f)
                    .SetEase(Ease.OutElastic)
                    .OnComplete(()=>
                    {
                        openLeaderboard_btn.transform.DOScale(Vector3.one, 0.3f);
                        close_btn.transform.DOScale(Vector3.one, 0.5f);
                        canSkip = true;
                    });
                if(previousTier < newTier) AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.rank_goupFinish);
                else AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.rank_same);
            });
    }

    public void OpenLeaderboard()
    {
        leaderboardManager.OpenLeaderboardAt(gameType);
    }

    public void Close()
    {
        if (gameObject.activeSelf == false) return;
        if(DOTween.IsTweening(tierBoard_ui.gameObject.transform)) return;
        if(!canSkip) return;
        
        BG.DOFade(0, 0.5f);
        tierBoard_ui.gameObject.transform.position = Vector3.zero;
        tierBoard_ui.gameObject.transform.DOLocalMoveY(2000, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
    }

    public void RankingBadgeClicked(int idx)
    {
        GameType gameType = (GameType)idx;
        gameObject.SetActive(true);
        StartCoroutine(ShowRankingUI(gameType, true));
    }

    public void SetUI(RankUIPage uiPage)
    {
        score_ui.SetActive(uiPage == RankUIPage.Score);
        login_ui.SetActive(uiPage == RankUIPage.Login);
        failed_ui.SetActive(uiPage == RankUIPage.Failed);
    }

    public void LogInBtnClicked()
    {
        StartCoroutine(leaderboardManager.GetDataFromServers());
    }
    
    public enum RankUIPage { Score, Login, Failed }
}
