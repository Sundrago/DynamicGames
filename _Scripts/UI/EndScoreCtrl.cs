using UnityEngine;
using TMPro;
using DG.Tweening;
using Firebase.Analytics;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Firebase;
using Firebase.Analytics;

public enum GameType {land, jump, build, shoot, Null};

public class EndScoreCtrl : MonoBehaviour
{
    [Title("LAND")]
    [SerializeField]
    private Land_GameManager land_manager;
    [SerializeField]
    private ReturnToMenu land_return;
    
    [Title("JUMP")]
    [SerializeField]
    private Jump_GameManager jump_manager;
    [SerializeField]
    private ReturnToMenu jump_return;
    
    [Title("BUILD")]
    [SerializeField]
    private Build_GameManager build_manager;
    [SerializeField]
    private ReturnToMenu build_return;
    
    [Title("SHOOT")]
    [SerializeField]
    private Shoot_GameManager shoot_manager;
    [SerializeField]
    private ReturnToMenu shoot_return;
    
    [Title("others..")]
    [SerializeField] SFXCTRL sfx;
    [SerializeField] TextMeshProUGUI score_ui, socre_text_ui;
    [SerializeField] TextMeshProUGUI  slider_score, slider_high_title, slider_high_score;
    [SerializeField] Slider slider;
    [SerializeField] Image  slider_indicator_high;
    [SerializeField] Image bg;
    [SerializeField] LeaderboardManger leaderboard;
    [SerializeField] Gacha_TicketsHolder gachaTicketsHolder;
    [SerializeField] Ranking_UI rankingUI;
    [SerializeField]
    private Button retartBtn, LeaderboardBtn, backtoMenuBtn;

    public static EndScoreCtrl Instance;

    [SerializeField] private PetEndScoreMotionCtrl petEndScoreMotionCtrl;
    [SerializeField] private NewHighScoreFX newHighScoreFX;

    private int curretBgm = -1;
    private GameType currentGameType;
    private bool newHighscore;
    private float startTime;
    private GameType startGameType;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void StartGame(GameType _gameType)
    {
        startTime = Time.time;
        startGameType = _gameType;
        print("startTime : " +startTime);
    }

    public void ShowScore(int score, GameType gameType)
    {
#if UNITY_IOS && !UNITY_EDITOR
        FirebaseAnalytics.LogEvent("Score", gameType.ToString()+"_score", score);
        if (startGameType == gameType) {  
            FirebaseAnalytics.LogEvent("Score", gameType.ToString()+"_playTime", Time.time-startTime);
        }
#endif
        print("duration : " + (Time.time-startTime));
        MoneyManager.Instance.ShowPanel();
        PlayerPrefs.SetInt("totalScoreCount", PlayerPrefs.GetInt("totalScoreCount") + 1);
        PlayerPrefs.Save();
        gameObject.SetActive(true);
        currentGameType = gameType;
        SetBtnActive(false);

        //bgm
        curretBgm = sfx.GetCurrentBgm();
        sfx.PlayBGM(4, false, 0.2f);
        AudioCtrl.Instance.PlaySFXbyTag(SFX_tag.scoreSlider);

        //save and load data
        int highScore = PlayerPrefs.GetInt("highscore_" + gameType.ToString());
        int previouseHighScore = highScore;
        if(score > highScore) PlayerPrefs.SetInt("highscore_" + gameType.ToString(), score);

        //Initailize
        score_ui.text = "0";
        slider_score.text = "0";
        slider_high_score.text = highScore.ToString();
        slider.value = 0;
        slider_high_title.DOFade(1f, 0f);
        slider_high_score.DOFade(1f, 0f);
        slider_indicator_high.DOFade(1f, 0f);
        socre_text_ui.text = "GAME OVER";
        
        //Init Pet Motions
        if (PetInGameManager.Instance.enterGameWithPet)
        {
            PetDialogueManager.PetScoreType petScoreType;
            if (score >= previouseHighScore) petScoreType = PetDialogueManager.PetScoreType.score_newBest;
            else if(score > previouseHighScore * 3f / 4f) petScoreType = PetDialogueManager.PetScoreType.score_excelent;
            else if(score > previouseHighScore *  2f / 4f) petScoreType = PetDialogueManager.PetScoreType.score_great;
            else if(score > previouseHighScore *  1f / 4f) petScoreType = PetDialogueManager.PetScoreType.score_good;
            else petScoreType = PetDialogueManager.PetScoreType.score_bad;
            
            petEndScoreMotionCtrl.Init(PetInGameManager.Instance.pet.type, petScoreType);
        }
        else
        {
            petEndScoreMotionCtrl.gameObject.SetActive(false);
        }

        //Sldier Anim
        float score_normal = (float)score/(float)highScore;
        if(score_normal > 0.7f) slider_high_title.DOFade(0f, 1f).SetDelay(0.5f);
        if(score_normal > 0.95f) {
            slider_high_score.DOFade(0f, 1f).SetDelay(0.5f);
            slider_indicator_high.DOFade(0f, 1f).SetDelay(0.5f);
        }

        slider.DOValue(score_normal / 3f * 2f, 0.8f)
            .SetEase(Ease.InExpo)
            .SetDelay(0.3f)
            .OnComplete(() => {
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

        //Title
        if(score > highScore) {
            socre_text_ui.gameObject.transform.DOShakeScale(1f).SetDelay(1.8f)
                .OnPlay(()=>{socre_text_ui.text = "NEW BEST!";});
            newHighscore = true;
            leaderboard.ReportScore(score, gameType);
        }
        newHighscore = false;

        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.DOLocalMoveY(-3000f, 0.5f)
            .SetEase(Ease.OutQuint)
            .From();
        bg.DOFade(0f, 0.5f)
            .From();
        
        //Tickets
        DOVirtual.DelayedCall(2f, () => { gachaTicketsHolder.InitTickets(score, previouseHighScore, gameType); });
    }

    public void TicketAnimFinished()
    {
        if (newHighscore)
        {
            StartCoroutine(rankingUI.ShowRankingUI(currentGameType, true));
            DOVirtual.DelayedCall(3.5f, () => {
                sfx.ChangeBGMVolume(1f, 3f);
                SetBtnActive(true);
            });
        }
        else
        {
            sfx.ChangeBGMVolume(1f, 2f);
            SetBtnActive(true);
        }
        
    }

    void UpdateScoreUI(int score) {
        score_ui.text = score.ToString();
        slider_score.text = score.ToString();
    }

    public void HideScore() {
        if(!gameObject.activeSelf) return;
        
        MoneyManager.Instance.HidePanel();
        gameObject.SetActive(false);
        if(curretBgm != -1) sfx.PlayBGM(curretBgm);
    }

    public void OpenLeaderboardBtnClicked() {
        leaderboard.OpenLeaderboardAt(currentGameType);
    }

    public void BackToHomeBtnClicked()
    {
        switch (currentGameType)
        {
            case GameType.build :
                build_return.ReturnToMenuClkcked();
                break;
            case GameType.jump :
                jump_return.ReturnToMenuClkcked();
                break;
            case GameType.land :
                land_return.ReturnToMenuClkcked();
                break;
            case GameType.shoot :
                shoot_return.ReturnToMenuClkcked();
                break;
        }
    }

    public void RestartBtnClicked()
    {
        switch (currentGameType)
        {
            case GameType.build :
                build_manager.RestartGame();
                break;
            case GameType.jump :
                jump_manager.StartGame();
                break;
            case GameType.land :
                land_manager.PlayAgain();
                break;
            case GameType.shoot :
                shoot_manager.RestartGame();
                break;
        }
    }

    public int GetHighScore(GameType gameType)
    {
        return PlayerPrefs.GetInt("highscore_" + gameType.ToString());
    }

    private void SetBtnActive(bool setActive)
    {
        retartBtn.interactable = setActive;
        LeaderboardBtn.interactable = setActive;
        backtoMenuBtn.interactable = setActive;
    }

    public void ShowNewHighFX()
    {
        newHighScoreFX.StartFX();
    }
}
