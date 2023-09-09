using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Threading.Tasks;

using TMPro;
//public enum leaderboard_ids {score_land, score_jump, score_build, score_shoot};

public class LeaderboardManger : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    private List<ILeaderboard> leaderboards = new List<ILeaderboard>();
    
    void Start()
    {
        Social.localUser.Authenticate(success => {
            if (success)
            {
                Debug.Log("Authentication successful");
                string userInfo = "Username: " + Social.localUser.userName +
                    "\nUser ID: " + Social.localUser.id +
                    "\nIsUnderage: " + Social.localUser.underage;
                Debug.Log(userInfo);

                text.text += userInfo + "\n";
                
                // create social leaderboard
                leaderboards = new List<ILeaderboard>();
                foreach(GameType gameType in Enum.GetValues(typeof(GameType)))
                {
                    string id = "score_" + gameType;
                    ILeaderboard leaderboard  = Social.CreateLeaderboard();
                    leaderboard.id = id;
                    leaderboard.LoadScores(result =>
                    {
                        if (!result) Debug.Log("Loading score in leaderboard " + id.ToString() + ": failed");
                        leaderboards.Add(leaderboard);
                
                        int rank = leaderboard.localUserScore.rank;
                        PlayerPrefs.SetInt("rank_" + gameType.ToString(), rank);
                        Debug.Log(id.ToString() + " rank : " + rank);
                        
                        foreach (IScore score in leaderboard.scores)
                        {
                            if(score.value == -1) text.text += "ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue + "\n";
                        }

                        text.text += id.ToString() + " rank : " + rank + "\n";
                    });
                }
                
            }
            else
                Debug.Log("Authentication failed");
        });

        
    }

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

    public void ReportScore(int score, GameType gameType)
    {
        string id = "score_" + gameType;
        Debug.Log("Reporting score " + score + " on leaderboard " + id.ToString());
        Social.ReportScore(score, id, success => {
            if (!success) Debug.Log("Reporting score " + score + " to leaderboard " + id.ToString() + ": failed");
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
        GetTotalPlayers2();
    }
    private async Task GetTotalPlayers()
    {
        text.text = "";
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            string id = "score_" + gameType;

            bool dataReceived = false;
            
            ILeaderboard leaderboard = GetLeaderboardByGameType(gameType);
            leaderboard.LoadScores(result =>
            {
                if (!result) Debug.Log("Loading score in leaderboard " + id.ToString() + ": failed");
            
                int rank = leaderboard.localUserScore.rank;
                PlayerPrefs.SetInt("rank_" + gameType.ToString(), rank);
                Debug.Log(id.ToString() + " rank : " + rank);
                text.text += id.ToString() + " rank : " + rank + "\n";

                if (leaderboard.scores.Length != 0)
                {
                    text.text += leaderboard.scores[0].userID + "\n";
                }
                
                // foreach (IScore score in leaderboard.scores)
                // {
                //     Debug.Log("ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue);
                //     text.text += "ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue + "\n";
                // }
                
                leaderboard.SetUserFilter(new string[]{"sunyong@exitstudio.net"});
                leaderboard.LoadScores(result =>
                {
                    if (!result) Debug.Log("Loading score in leaderboard " + id.ToString() + ": failed");
                    
                    foreach (IScore score in leaderboard.scores)
                    {
                        Debug.Log("ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue);
                        text.text += "ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue + "\n";
                    }
                    dataReceived = true;
                });
                
                dataReceived = true;
            });
            
            while (!dataReceived)
            {
                await Task.Delay(200);
            }

        }

        await GetTotalPlayers2();
    }
    
    private async Task GetTotalPlayers2()
    {
        text.text = "";
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            string id = "score_" + gameType;

            bool dataReceived = false;
            ILeaderboard leaderboard  = Social.CreateLeaderboard();
            leaderboard.id = id;
            leaderboard.SetUserFilter(new string[]{"1494225810072163020", "169422923966517408", Social.localUser.id});
            leaderboard.LoadScores(result =>
            {
                if (!result) Debug.Log("Loading score in leaderboard " + id.ToString() + ": failed");
                
                foreach (IScore score in leaderboard.scores)
                {
                    Debug.Log("ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue);
                    text.text += "ID : " + score.userID + ", Rank : " + score.rank + ", Score " + score.formattedValue + "\n";
                }
                dataReceived = true;
            
            dataReceived = true;
            });
            
            while (!dataReceived)
            {
                await Task.Delay(200);
            }
        }
    }
}
