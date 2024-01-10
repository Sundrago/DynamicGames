using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.Networking;
using MyUtility;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using Random = System.Random;
//public enum leaderboard_ids {score_land, score_jump, score_build, score_shoot};

public class LeaderboardManger : MonoBehaviour
{
    [SerializeField]
    private Ranking_UI rangking_ui;
    private const string dataURL = "https://data.sundragon.net/dynamic_games_data.csv";
    [SerializeField]
    private RankingManager rankingManager;
    [SerializeField]
    private TextMeshProUGUI DebugText_ui;

    private List<ILeaderboard> leaderboards;
    //private Dictionary<string, string> CSVData = new Dictionary<string, string>();

    public enum LoadStatus { loading, success, fail }
    public LoadStatus status;
    private LoadStatus gameCenterStatus, sundragonNetStatus;
    private const float timeoutTime = 10f;

    public bool debug_randomRank = false;
    
    public ILeaderboard GetLeaderboardByGameType(GameType gameType)
    {
        foreach (ILeaderboard leaderboard in leaderboards)
        {
            string id = "score_" + gameType;
            if (leaderboard.id == id) return leaderboard;
        }
        Debug.Log("Leaderboard not found : " + gameType);
        return null;
    }

    [Button]
    public void ReportScore(int score, GameType gameType)
    {
        string id = "score_" + gameType;
        Debug.Log("Reporting score " + score + " on leaderboard " + id.ToString());
        Social.ReportScore(score, id, success => {
            if (!success)
            {
                // PlayerPrefs.SetInt("rank_" + gameType, -1);
                Debug.Log("Reporting score " + score + " to leaderboard " + id.ToString() + ": failed");
                return;
            }
            
            ILeaderboard leaderboard = GetLeaderboardByGameType(gameType);
            leaderboard.LoadScores(success => {
                if (!success)
                {
                    Debug.Log("Loading score " + score + " to leaderboard " + id.ToString() + ": failed");
                    return;
                }
                int rank = leaderboard.localUserScore.rank;
                rangking_ui.gameObject.SetActive(true);
                StartCoroutine(rangking_ui.ShowRankingUI(gameType, true));
                
                if(rank < PlayerPrefs.GetInt("rank_" + gameType))
                {
                    PlayerPrefs.SetInt("rank_" + gameType, rank);
                }
            });
        });
    }

    public void OpenLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
    
    public void OpenLeaderboardAt(GameType gameType)
    {
        string id = "score_" + gameType;
        GameCenterPlatform.ShowLeaderboardUI(id.ToString(), TimeScope.AllTime);
    }

    public void Start()
    {
        debug_randomRank = false;
        StartCoroutine(GetDataFromServers());
    }
    
    public IEnumerator GetDataFromServers()
    {
        DebugText_ui.text = "";
        status = LoadStatus.loading;
        
        gameCenterStatus = LoadStatus.loading;
        sundragonNetStatus = LoadStatus.loading;
        
        StartCoroutine(GetDataFromGameCenter());
        StartCoroutine(GetDataFromSunDragonNet());

        float startTime = Time.time;
        while (status == LoadStatus.loading && (gameCenterStatus == LoadStatus.loading || sundragonNetStatus == LoadStatus.loading))
        {
            if (Time.time - startTime > timeoutTime) status = LoadStatus.fail;
            yield return new WaitForSeconds(0.2f);
        }

        status = (gameCenterStatus == LoadStatus.success && sundragonNetStatus == LoadStatus.success) ? LoadStatus.success : LoadStatus.fail;
        Debug.Log("BothSatus : " + status.ToString());

        if (status == LoadStatus.success)
        {
            StartCoroutine(rankingManager.UpdatetRanks());
            rangking_ui.Close();
        }
    }
    
    IEnumerator GetDataFromGameCenter()
    {
        string debugString = "";
        leaderboards = new List<ILeaderboard>();
        
        Social.localUser.Authenticate(success => {
            if (success)
            {
                rangking_ui.Close();
                
                debugString = "GameCenter Authentication Success" + 
                              "\nUsername: " + Social.localUser.userName + 
                              "\nUser ID: " + Social.localUser.id + 
                              "\nIsUnderage: " + Social.localUser.underage;
                
                leaderboards = new List<ILeaderboard>();
                foreach(GameType gameType in Enum.GetValues(typeof(GameType)))
                {
                    string id = "score_" + gameType;
                    ILeaderboard leaderboard  = Social.CreateLeaderboard();
                    leaderboard.id = id;
                    leaderboard.LoadScores(result =>
                    {
                        if (!result)
                        {
                            debugString += "\nLoading score in leaderboard " + id.ToString() + ": failed";
                            PlayerPrefs.SetInt("rank_" + gameType.ToString(), -1);
                            return;
                        }
                        
                        leaderboards.Add(leaderboard);
                        int rank = leaderboard.localUserScore.rank;
                        if(debug_randomRank) rank = UnityEngine.Random.Range(100,0);
                        PlayerPrefs.SetInt("rank_" + gameType.ToString(), rank);
                        
                        int highScorePref = PlayerPrefs.GetInt("highscore_" + gameType);
                        int highScoreGC = (int)leaderboard.localUserScore.value;
                        if(highScorePref < highScoreGC) PlayerPrefs.SetInt("highscore_" + gameType, highScoreGC);

                        if (leaderboards.Count == Enum.GetValues(typeof(GameType)).Length)
                            gameCenterStatus = LoadStatus.success;
                    });
                }
            }
            else
            {
                rangking_ui.SetUI(Ranking_UI.RankUIPage.Failed);
                gameCenterStatus = LoadStatus.fail;
                debugString = "GameCenter Authentication failed";
            }
        });

        float startTime = Time.time;
        while (gameCenterStatus == LoadStatus.loading)
        {
            if (Time.time - startTime > timeoutTime)
            {
                debugString = "GameCenter Authentication failed : Timeout";
                gameCenterStatus = LoadStatus.fail;
            }
            yield return new WaitForSeconds(0.2f);
        }

        DebugText_ui.text += debugString;
        Debug.Log(debugString);
    }

    
    IEnumerator GetDataFromSunDragonNet()
    {
        string debugString = "\nConnecting to sundragon.net : ";
        UnityWebRequest request = UnityWebRequest.Get(dataURL);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            debugString += "failed\n" +
                           request.error;
            sundragonNetStatus = LoadStatus.fail;
            rangking_ui.SetUI(Ranking_UI.RankUIPage.Failed);
        }
        else
        {
            debugString += "success!";
            
            string data = request.downloadHandler.text;
            string[] rows = data.Split('\n');
            
            foreach(string row in rows)
            {
                string[] cols = row.Split(',');
                if(cols[0] == "") continue;
                //if(!CSVData.ContainsKey(cols[0])) CSVData.Add(cols[0], cols[1]);
                PlayerPrefs.SetString(cols[0], cols[1]);
                debugString += ("\n " + cols[0] + " : " + cols[1]);
            }
            sundragonNetStatus = LoadStatus.success;
        }
        
        DebugText_ui.text += debugString;
        Debug.Log(debugString);
    }
}