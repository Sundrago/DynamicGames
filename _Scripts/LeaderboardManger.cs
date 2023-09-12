using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.Networking;
using MyUtility;
using System.Threading.Tasks;

using TMPro;
using Random = System.Random;
//public enum leaderboard_ids {score_land, score_jump, score_build, score_shoot};

public class LeaderboardManger : MonoBehaviour
{
    private const string dataURL = "https://data.sundragon.net/dynamic_games_data.csv";
    [SerializeField]
    private RankingManager rangkingManager;
    [SerializeField]
    private TextMeshProUGUI DebugText_ui;
    
    private List<ILeaderboard> leaderboards = new List<ILeaderboard>();
    //private Dictionary<string, string> CSVData = new Dictionary<string, string>();

    public enum LoadStatus { loading, success, fail }
    public LoadStatus status;
    private LoadStatus gameCenterStatus, sundragonNetStatus;
    private const float timeoutTime = 10f;

    private bool debug_randomRank = false;
    
    ILeaderboard GetLeaderboardByGameType(GameType gameType)
    {
        foreach (ILeaderboard leaderboard in leaderboards)
        {
            string id = "score_" + gameType;
            if (leaderboard.id == id) return leaderboard;
        }
        Debug.Log("Leaderboard not found : " + gameType);
        return null;
    }

    public void ReportScore(int score, GameType gameType, Action<int> returnMethod = null)
    {
        string id = "score_" + gameType;
        Debug.Log("Reporting score " + score + " on leaderboard " + id.ToString());
        Social.ReportScore(score, id, success => {
            if (!success)
            {
                Debug.Log("Reporting score " + score + " to leaderboard " + id.ToString() + ": failed");
                return;
            }

            if (returnMethod != null)
            {
                ILeaderboard leaderboard = GetLeaderboardByGameType(gameType);
                leaderboard.LoadScores(result => {
                    if (!result)
                    {
                        //debugString += "\nLoading score in leaderboard " + id.ToString() + ": failed";
                        //PlayerPrefs.SetInt("rank_" + gameType.ToString(), -1);
                    }

                    int rank = leaderboard.localUserScore.rank;
                    if (debug_randomRank) rank = UnityEngine.Random.Range(100, 0);
                    // PlayerPrefs.SetInt("rank_" + gameType.ToString(), rank);
                    // debugString += "\n Rank_" + gameType + " : " + rank;
                });
            }
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

    public void GetTotalPlayersBtn()
    {
        debug_randomRank = true;
        StartCoroutine(GetDataFromServers());
    }

    public void Start()
    {
        debug_randomRank = false;
        StartCoroutine(GetDataFromServers());
    }
    
    private IEnumerator GetDataFromServers()
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
        Debug.Log(status.ToString());
        
        if(status == LoadStatus.success) StartCoroutine(rangkingManager.UpdatetRanks());
    }
    
    IEnumerator GetDataFromGameCenter()
    {
        string debugString = "";
        
        Social.localUser.Authenticate(success => {
            if (success)
            {
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
                        debugString += "\n Rank_" + gameType + " : " + rank ;

                        if (leaderboards.Count == Enum.GetValues(typeof(GameType)).Length)
                            gameCenterStatus = LoadStatus.success;
                    });
                }
            }
            else
            {
                gameCenterStatus = LoadStatus.fail;
                debugString = "GameCenter Authentication failed";
            }
        });

        float startTime = Time.time;
        while (gameCenterStatus == LoadStatus.loading)
        {
            if (Time.time - startTime > timeoutTime)
            {
                debugString = "ameCenter Authentication failed : Timeout";
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