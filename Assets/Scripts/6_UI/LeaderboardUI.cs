using System.Collections;
using DG.Tweening;
using DynamicGames.MiniGames;
using DynamicGames.System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DynamicGames.UI
{
    /// <summary>
    /// Manages the user interface for the leaderboard.
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        public enum RankUIPage
        {
            Score,
            Login,
            Failed
        }

        [Header("Managers and Controllers")] [SerializeField]
        private RankingManager rankingManager;

        [SerializeField] private LeaderboardManger leaderboardManager;

        [FormerlySerializedAs("sfx")] [SerializeField]
        private SfxController sfxController;

        [FormerlySerializedAs("slider_ui")] [Header("UI Elements")] [SerializeField]
        private Slider progressSlider;

        [FormerlySerializedAs("score_text_ui")] [SerializeField]
        private TextMeshProUGUI scoreText;

        [FormerlySerializedAs("title_ui")] [SerializeField]
        private TextMeshProUGUI titleText;

        [FormerlySerializedAs("tier_image_ui")] [SerializeField]
        private Image tierImage;

        [FormerlySerializedAs("BG")] [SerializeField]
        private Image bgImage;

        [FormerlySerializedAs("tier_icon_group")] [SerializeField]
        private RectTransform tierIconGroup;

        [SerializeField] private RectTransform[] tierIconRectTransforms;

        [FormerlySerializedAs("openLeaderboard_btn")] [SerializeField]
        private Button openLeaderboardButton;

        [FormerlySerializedAs("close_btn")] [SerializeField]
        private Button closeButton;

        [FormerlySerializedAs("tierBoard_ui")] [SerializeField]
        private Transform tierBoard;

        [SerializeField] private GameObject score_ui, login_ui, failed_ui;
        [SerializeField] private float tierIconGroupOffsetY;
        private bool canSkipAnimation;

        private GameType gameType;

        [Button]
        public IEnumerator ShowRankingUI(GameType gameType, bool forceShow = false)
        {
            this.gameType = gameType;
            var previousRank = PlayerPrefs.GetInt("previousRank" + this.gameType);
            var newRank = PlayerPrefs.GetInt("rank_" + this.gameType);

            if (newRank == -1)
            {
                SetUI(RankUIPage.Login);
                yield break;
            }

            SetUI(RankUIPage.Score);

            if (ShouldCancelRankingUI(forceShow, previousRank, newRank))
            {
                sfxController.ChangeBGMVolume();
                gameObject.SetActive(false);
                yield break;
            }

            var totalPlayerCount = rankingManager.GetTotalPlayerCountByGameType(this.gameType);
            if (previousRank > totalPlayerCount) totalPlayerCount = previousRank;
            if (newRank > totalPlayerCount) totalPlayerCount = newRank;

            var previousRankInPercent = rankingManager.GetRankInPercent(previousRank, totalPlayerCount);
            var newRankInPercent = rankingManager.GetRankInPercent(newRank, totalPlayerCount);
            var previousTier = rankingManager.GetTiersFromRank(previousRankInPercent);
            var newTier = rankingManager.GetTiersFromRank(newRankInPercent);

            var startPosY = GetTierIconPositionY(previousTier);
            var targetPosY = GetTierIconPositionY(newTier);
            var targetMidPosY = (targetPosY + startPosY) / 2f;
            var duration = previousTier < newTier ? 4f : 1f;

            SetupInitialState(startPosY, previousRankInPercent, newRank);
            SetupPanelAnimation();
            yield return new WaitForSeconds(0.4f);
            SetupSliderAnimation(previousTier, newTier, previousRankInPercent, newRankInPercent, duration);
            SetupTierIconAnimation(targetPosY, targetMidPosY, duration, previousTier, newTier);
        }

        private bool ShouldCancelRankingUI(bool forceShow, int previousRank, int newRank)
        {
            return !forceShow && (DOTween.IsTweening(tierBoard.gameObject.transform) || previousRank == newRank);
        }

        private void SetupInitialState(float startPosY, float previousRankInPercent, int newRank)
        {
            titleText.text = leaderboardManager.GetLeaderboardByGameType(gameType).title;
            canSkipAnimation = false;
            closeButton.transform.localScale = Vector3.zero;
            openLeaderboardButton.gameObject.transform.localScale = Vector3.zero;
            bgImage.color = new Color(0, 0, 0, 0);
            tierBoard.gameObject.transform.localPosition = Vector3.zero;

            tierIconGroup.transform.localPosition = new Vector3(0, startPosY, 0);
            progressSlider.value = previousRankInPercent / 100f;
            PlayerPrefs.SetInt("previousRank" + gameType, newRank);
            gameObject.SetActive(true);
        }

        private void SetupPanelAnimation()
        {
            bgImage.DOFade(0.7f, 0.4f);
            tierBoard.gameObject.transform.DOLocalMoveY(2000, 0.5f)
                .From()
                .SetEase(Ease.OutBack);
        }

        private float GetTierIconPositionY(Tiers tier)
        {
            if ((int)tier >= tierIconRectTransforms.Length) return tierIconRectTransforms.Length - 1;
            return tierIconRectTransforms[(int)tier].anchoredPosition.y * -1f + tierIconGroupOffsetY;
        }

        private void SetupSliderAnimation(Tiers previousTier, Tiers newTier, float previousRankInPercent,
            float newRankInPercent, float duration)
        {
            if (DOTween.IsTweening(progressSlider)) DOTween.Kill(progressSlider);

            progressSlider.DOValue(newRankInPercent / 100f, duration)
                .SetEase(Ease.InOutExpo)
                .OnUpdate(() =>
                {
                    var percent = Mathf.Round(progressSlider.value * 1000f) / 10f;
                    scoreText.text = percent + "%";
                });
        }

        private void SetupTierIconAnimation(float targetPosY, float targetMidPosY, float duration, Tiers previousTier,
            Tiers newTier)
        {
            if (DOTween.IsTweening(tierIconGroup.transform)) DOTween.Kill(tierIconGroup.transform);

            tierIconGroup.DOLocalMove(new Vector3(0, targetMidPosY, 0), duration / 3f)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    tierIconGroup.DOLocalMove(new Vector3(0, targetPosY, 0), duration / 3f * 2f)
                        .SetEase(Ease.OutElastic)
                        .OnComplete(() => { CanSkipAnimation(); });
                    if (previousTier < newTier) AudioManager.Instance.PlaySfxByTag(SfxTag.RankGroupFinish);
                    else AudioManager.Instance.PlaySfxByTag(SfxTag.RankUnChanged);
                });
        }

        private void CanSkipAnimation()
        {
            closeButton.transform.DOScale(Vector3.one, 0.5f);
            openLeaderboardButton.transform.DOScale(Vector3.one, 0.3f);
            canSkipAnimation = true;
            sfxController.ChangeBGMVolume();
        }

        public void OpenLeaderboard()
        {
            leaderboardManager.OpenLeaderboardAt(gameType);
        }

        public void Close()
        {
            if (gameObject.activeSelf == false) return;
            if (DOTween.IsTweening(tierBoard.gameObject.transform)) return;
            if (!canSkipAnimation)
            {
                DOVirtual.DelayedCall(2f, () => { canSkipAnimation = true; });
                return;
            }

            bgImage.DOFade(0, 0.5f);
            tierBoard.gameObject.transform.position = Vector3.zero;
            tierBoard.gameObject.transform.DOLocalMoveY(2000, 0.5f)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => { gameObject.SetActive(false); });
        }

        [Button]
        public void RankingBadgeClicked(int idx)
        {
            var gameType = (GameType)idx;
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
    }
}