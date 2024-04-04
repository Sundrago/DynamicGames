using Core.Gacha;
using Core.Pet;
using Core.System;
using DG.Tweening;
using Games;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;
#if UNITY_IOS && !UNITY_EDITOR
using Firebase;
using Firebase.Analytics;
#endif

namespace Core.UI
{
    public class GameScoreManager : SerializedMonoBehaviour
    {
        [Title("Game Managers")]
        // [SerializeField] private Dictionary<GameType, MiniGame> gameManagers;
        [Title("Managers and Controllers")]
        [SerializeField]
        private PetEndScoreMotionCtrl petEndScoreMotionCtrl;

        [SerializeField] private NewHighScoreFX newHighScoreFX;

        [FormerlySerializedAs("sfx")] [SerializeField]
        private SfxController sfxController;

        [FormerlySerializedAs("leaderboard")] [SerializeField]
        private LeaderboardManger leaderboardManger;

        [SerializeField] private TicketsController ticketsController;

        [FormerlySerializedAs("rankingUI")] [SerializeField]
        private LeaderboardUI leaderboardUI;

        [Title("UI Components")] [SerializeField]
        private TextMeshProUGUI score_ui, socre_text_ui, slider_score, slider_high_title, slider_high_score;

        [SerializeField] private Slider slider;
        [SerializeField] private Image slider_indicator_high, bg;

        [FormerlySerializedAs("retartBtn")] [SerializeField]
        private Button restartButton;

        [FormerlySerializedAs("leaderboardBtn")] [SerializeField]
        private Button leaderboardButton;

        [FormerlySerializedAs("backtoMenuBtn")] [SerializeField]
        private Button backToMenuButton;

        private GameType currentGameType;

        private int curretBgm = -1;
        private bool newHighscore;

        private GameType startGameType;
        private float startTime;

        public static GameScoreManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void StartGame(GameType gameType)
        {
            startTime = Time.time;
            startGameType = gameType;
        }

        public void ShowScore(int score, GameType gameType)
        {
            InitializeScore(score, gameType);
            PlaySfx();

            var highScore = PlayerPrefs.GetInt("highscore_" + gameType);
            var previousHighScore = highScore;
            if (score > highScore) PlayerPrefs.SetInt("highscore_" + gameType, score);

            ResetUIElements(highScore);
            InitPetMotion(score, previousHighScore);
            AnimateSlider(score, highScore);
            UpdateTitle(score, highScore, gameType);
            AnimatePanel();

            DOVirtual.DelayedCall(2f, () => { ticketsController.InitTickets(score, previousHighScore, gameType); });
        }

        private void InitializeScore(int score, GameType gameType)
        {
#if UNITY_IOS && !UNITY_EDITOR
        LogFirebaseEvents(score, gameType);
#endif
            MoneyManager.Instance.ShowPanel();
            PlayerData.SetInt(DataKey.totalScoreCount, PlayerData.GetInt(DataKey.totalScoreCount) + 1);

            gameObject.SetActive(true);
            currentGameType = gameType;
            SetBtnActive(false);
        }

        private void LogFirebaseEvents(int score, GameType gameType)
        {
#if UNITY_IOS && !UNITY_EDITOR
        FirebaseAnalytics.LogEvent("Score", gameType.ToString()+"_score", score);
        if (startGameType == gameType) {  
            FirebaseAnalytics.LogEvent("Score", gameType.ToString()+"_playTime", Time.time-startTime);
        }
        FirebaseAnalytics.LogEvent("GamePlayed", "GamePlayed", gameType.ToString());
        // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, new Parameter("GameType", score));
        // FirebaseAnalytics.LogEvent("PlayTime", new Parameter("GameType", Time.time-startTime));
#endif
        }

        private void PlaySfx()
        {
            curretBgm = sfxController.GetCurrentBgmIndex();
            sfxController.PlayBGM(4, false, 0.2f);
            AudioManager.Instance.PlaySfxByTag(SfxTag.ShowScore);
            AudioManager.Instance.PlaySfxByTag(SfxTag.ScoreSlider);
        }

        private void ResetUIElements(int highScore)
        {
            score_ui.text = "0";
            slider_score.text = "0";
            slider_high_score.text = highScore.ToString();
            slider.value = 0;
            slider_high_title.DOFade(1f, 0f);
            slider_high_score.DOFade(1f, 0f);
            slider_indicator_high.DOFade(1f, 0f);
            socre_text_ui.text = "GAME OVER";
        }

        private void InitPetMotion(int score, int previousHighScore)
        {
            if (PetInGameManager.Instance.EnterGameWithPet)
            {
                PetScoreType petScoreType;
                if (score >= previousHighScore) petScoreType = PetScoreType.NewBest;
                else if (score > previousHighScore * 3f / 4f) petScoreType = PetScoreType.Excellent;
                else if (score > previousHighScore * 2f / 4f) petScoreType = PetScoreType.Great;
                else if (score > previousHighScore * 1f / 4f) petScoreType = PetScoreType.Good;
                else petScoreType = PetScoreType.Bad;

                petEndScoreMotionCtrl.Init(PetInGameManager.Instance.petObject.type, petScoreType);
            }
            else
            {
                petEndScoreMotionCtrl.gameObject.SetActive(false);
            }
        }

        private void AnimateSlider(int score, int highScore)
        {
            var score_normal = score / (float)highScore;
            if (score_normal > 0.7f) slider_high_title.DOFade(0f, 1f).SetDelay(0.5f);
            if (score_normal > 0.95f)
            {
                slider_high_score.DOFade(0f, 1f).SetDelay(0.5f);
                slider_indicator_high.DOFade(0f, 1f).SetDelay(0.5f);
            }

            slider.DOValue(score_normal / 3f * 2f, 0.8f)
                .SetEase(Ease.InExpo)
                .SetDelay(0.3f)
                .OnComplete(() =>
                {
                    slider.DOValue(score_normal, 0.4f)
                        .SetEase(Ease.OutElastic);
                });

            DOVirtual.Int(0, score, 1.25f, UpdateScoreUI)
                .SetDelay(0.3f)
                .SetEase(Ease.InOutCubic);
            score_ui.DOScale(0.4f, 0.75f)
                .SetEase(Ease.OutBack)
                .SetDelay(0.3f)
                .From();
        }

        private void UpdateTitle(int score, int highScore, GameType gameType)
        {
            if (score > highScore)
            {
                socre_text_ui.gameObject.transform.DOShakeScale(1f).SetDelay(1.8f)
                    .OnPlay(() => { socre_text_ui.text = "NEW BEST!"; });
                newHighscore = true;
                leaderboardManger.ReportScore(score, gameType);
            }

            newHighscore = false;
        }

        private void AnimatePanel()
        {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.DOLocalMoveY(-3000f, 0.5f)
                .SetEase(Ease.OutQuint)
                .From();
            bg.DOFade(0f, 0.5f)
                .From();
        }

        public void TicketAnimFinished()
        {
            if (newHighscore)
            {
                StartCoroutine(leaderboardUI.ShowRankingUI(currentGameType, true));
                DOVirtual.DelayedCall(3.5f, () =>
                {
                    sfxController.ChangeBGMVolume();
                    SetBtnActive(true);
                });
            }
            else
            {
                sfxController.ChangeBGMVolume(1f, 2f);
                SetBtnActive(true);
            }
        }

        private void UpdateScoreUI(int score)
        {
            score_ui.text = score.ToString();
            slider_score.text = score.ToString();
        }

        public void HideScore()
        {
            if (!gameObject.activeSelf) return;

            MoneyManager.Instance.HidePanel();
            gameObject.SetActive(false);
            if (curretBgm != -1) sfxController.PlayBGM(curretBgm);
        }

        public void OpenLeaderboardBtnClicked()
        {
            leaderboardManger.OpenLeaderboardAt(currentGameType);
        }

        public void BackToHomeBtnClicked()
        {
            MiniGamesManager.Instance.ExitGame(currentGameType);
        }

        public void RestartBtnClicked()
        {
            MiniGamesManager.Instance.RestartGame(currentGameType);
        }

        public int GetHighScore(GameType gameType)
        {
            return PlayerPrefs.GetInt("highscore_" + gameType);
        }

        private void SetBtnActive(bool setActive)
        {
            restartButton.interactable = setActive;
            leaderboardButton.interactable = setActive;
            backToMenuButton.interactable = setActive;
        }

        public void ShowNewHighFX()
        {
            newHighScoreFX.StartFX();
        }
    }
}