using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public enum leaderboard_ids {score_land, score_jump, score_build, score_shoot};

public class LeaderboardManger : MonoBehaviour
{
    private List<ILeaderboard> leaderboards = new List<ILeaderboard>();

    // Start is called before the first frame update
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
            }
            else
                Debug.Log("Authentication failed");
        });

        // create social leaderboard
        foreach(string id in System.Enum.GetNames(typeof(leaderboard_ids))) {
            ILeaderboard leaderboard  = Social.CreateLeaderboard();
            leaderboard.id = id;
            leaderboard.LoadScores(result =>
            {
                Debug.Log("Received " + leaderboard.scores.Length + " scores");
                leaderboards.Add(leaderboard);
                // foreach (IScore score in leaderboard.scores)
                //     Debug.Log(score);
            });
        }
    }

    public void ReportScore(int score, leaderboard_ids id)
    {
        Debug.Log("Reporting score " + score + " on leaderboard " + id.ToString());
        Social.ReportScore(score, id.ToString(), success => {
            Debug.Log(success ? "Reported score successfully" : "Failed to report score");
        });
    }

    public void OpenLeaderboard()
    {
        Social.ShowLeaderboardUI();
        
    }
    
    public void OpenLeaderboardAt(leaderboard_ids id) {
        GameCenterPlatform.ShowLeaderboardUI(id.ToString(), TimeScope.AllTime);
    }
}
