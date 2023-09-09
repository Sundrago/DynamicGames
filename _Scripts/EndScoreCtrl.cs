using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public enum GameType {land, jump, build, shoot};

public class EndScoreCtrl : MonoBehaviour
{
    [SerializeField] SFXCTRL sfx;
    [SerializeField] TextMeshProUGUI score_ui, socre_text_ui;
    [SerializeField] TextMeshProUGUI slider_score_title, slider_score, slider_high_title, slider_high_score;
    [SerializeField] Slider slider;
    [SerializeField] Image slider_indicator, slider_indicator_high;
    [SerializeField] Image bg;
    [SerializeField] LeaderboardManger leaderboard;

    private int curretBgm = -1;
    private GameType currentGameType;
    
    public void ShowScore(int score, GameType gameType){
        gameObject.SetActive(true);
        currentGameType = gameType;

        //bgm
        curretBgm = sfx.GetCurrentBgm();
        sfx.PlayBGM(4);

        //save and load data
        int highScore = PlayerPrefs.GetInt("highscore_" + gameType.ToString());
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

        //Sldier Anim
        float score_normal = (float)score/(float)highScore;
        if(score_normal > 0.7f) slider_high_title.DOFade(0f, 1f).SetDelay(0.5f);
        if(score_normal > 0.95f) {
            slider_high_score.DOFade(0f, 1f).SetDelay(0.5f);
            slider_indicator_high.DOFade(0f, 1f).SetDelay(0.5f);
        }

        slider.DOValue(score_normal, 1.5f)
            .SetEase(Ease.InOutCubic)
            .SetDelay(0.3f);

        DOVirtual.Int(0, score, 1.5f, UpdateScoreUI)
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

            leaderboard.ReportScore(score, gameType);
        }

        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.DOLocalMoveY(-3000f, 0.5f)
            .SetEase(Ease.OutQuint)
            .From();
        bg.DOFade(0f, 0.5f)
            .From();
    }

    void UpdateScoreUI(int score) {
        score_ui.text = score.ToString();
        slider_score.text = score.ToString();
    }

    public void HideScore() {
        if(curretBgm == -1) return;
        sfx.PlayBGM(curretBgm);
        gameObject.SetActive(false);
    }

    public void OpenLeaderboardAt() {
        leaderboard.OpenLeaderboardAt(currentGameType);
    }

    public int GetHighScore(GameType gameType)
    {
        return PlayerPrefs.GetInt("highscore_" + gameType.ToString());
    }
}
