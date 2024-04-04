using System;
using System.Collections;
using System.Collections.Generic;
using Core.UI;
using Games;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

namespace Core.System{
    public class LeaderboardManger : MonoBehaviour
    {
        [SerializeField] private LeaderboardUI rangking_ui;
        [SerializeField] private RankingManager rankingManager;
        [SerializeField] private TextMeshProUGUI DebugText_ui;

        private const string DataURL = "https://data.sundragon.net/dynamic_games_data.csv";
        private const float TimeoutTime = 10f;

        public LoadStatus status;
        public bool debug_randomRank { get; set; }
        private LoadStatus gameCenterStatus, sundragonNetStatus;
        private List<ILeaderboard> leaderboards;

        public void Start()
        {
            debug_randomRank = false;
            StartCoroutine(GetDataFromServers());
        }

        public ILeaderboard GetLeaderboardByGameType(GameType gameType)
        {
            foreach (var leaderboard in leaderboards)
            {
                var id = "score_" + gameType;
                if (leaderboard.id == id) return leaderboard;
            }

            Debug.Log("Leaderboard not found : " + gameType);
            return null;
        }

        public void ReportScore(int score, GameType gameType)
        {
            var id = "score_" + gameType;
            Social.ReportScore(score, id, success =>
            {
                if (!success)
                {
                    Debug.Log("Reporting score " + score + " to leaderboard " + id + ": failed");
                    return;
                }

                var leaderboard = GetLeaderboardByGameType(gameType);
                leaderboard.LoadScores(success =>
                {
                    if (!success)
                    {
                        Debug.Log("Loading score " + score + " to leaderboard " + id + ": failed");
                        return;
                    }

                    var rank = leaderboard.localUserScore.rank;
                    rangking_ui.gameObject.SetActive(true);
                    StartCoroutine(rangking_ui.ShowRankingUI(gameType, true));

                    if (rank < PlayerPrefs.GetInt("rank_" + gameType)) PlayerPrefs.SetInt("rank_" + gameType, rank);
                });
            });
        }

        public void OpenLeaderboard()
        {
            Social.ShowLeaderboardUI();
        }

        public void OpenLeaderboardAt(GameType gameType)
        {
            var id = "score_" + gameType;
#if UNITY_IOS
            GameCenterPlatform.ShowLeaderboardUI(id, TimeScope.AllTime);
#endif
        }

        public IEnumerator GetDataFromServers()
        {
            ResetServerStatuses();

            var startTime = Time.time;
            while (status == LoadStatus.Loading &&
                   (gameCenterStatus == LoadStatus.Loading || sundragonNetStatus == LoadStatus.Loading))
            {
                if (Time.time - startTime > TimeoutTime) status = LoadStatus.Failed;
                yield return new WaitForSeconds(0.2f);
            }

            status = gameCenterStatus == LoadStatus.Success && sundragonNetStatus == LoadStatus.Success
                ? LoadStatus.Success
                : LoadStatus.Failed;

            if (status == LoadStatus.Success)
            {
                StartCoroutine(rankingManager.UpdateRanks());
                rangking_ui.Close();
            }
        }

        private void ResetServerStatuses()
        {
            DebugText_ui.text = "";
            status = LoadStatus.Loading;
            gameCenterStatus = LoadStatus.Loading;
            sundragonNetStatus = LoadStatus.Loading;

            StartCoroutine(GetDataFromGameCenter());
            StartCoroutine(GetDataFromSunDragonNet());
        }

        private IEnumerator GetDataFromGameCenter()
        {
            var debugString = "";
            leaderboards = new List<ILeaderboard>();

            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    HandleAuthSuccess(ref debugString);
                }
                else
                {
                    HandleAuthFailed(ref debugString);
                }
            });

            var startTime = Time.time;
            while (gameCenterStatus == LoadStatus.Loading)
            {
                if (Time.time - startTime > TimeoutTime)
                {
                    debugString = "GameCenter Authentication failed : Timeout";
                    gameCenterStatus = LoadStatus.Failed;
                }

                yield return new WaitForSeconds(0.2f);
            }

            DebugText_ui.text += debugString;
            Debug.Log(debugString);
        }


        private void HandleAuthSuccess(ref string debugString)
        {
            rangking_ui.Close();

            debugString = "GameCenter Authentication Success" +
                          "\nUsername: " + Social.localUser.userName +
                          "\nUser ID: " + Social.localUser.id +
                          "\nIsUnderage: " + Social.localUser.underage;

            leaderboards = new List<ILeaderboard>();
            foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
            {
                var id = "score_" + gameType;
                var leaderboard = Social.CreateLeaderboard();
                leaderboard.id = id;
                leaderboard.LoadScores(result =>
                {
                    if (!result)
                    {
                        PlayerPrefs.SetInt("rank_" + gameType, -1);
                        return;
                    }

                    leaderboards.Add(leaderboard);
                    var rank = leaderboard.localUserScore.rank;
                    if (debug_randomRank) rank = Random.Range(100, 0);
                    PlayerPrefs.SetInt("rank_" + gameType, rank);

                    var highScorePref = PlayerPrefs.GetInt("highscore_" + gameType);
                    var highScoreGC = (int)leaderboard.localUserScore.value;
                    if (highScorePref < highScoreGC) PlayerPrefs.SetInt("highscore_" + gameType, highScoreGC);

                    if (leaderboards.Count == Enum.GetValues(typeof(GameType)).Length)
                        gameCenterStatus = LoadStatus.Success;
                });
            }
        }

        private void HandleAuthFailed(ref string debugString)
        {
            rangking_ui.SetUI(LeaderboardUI.RankUIPage.Failed);
            gameCenterStatus = LoadStatus.Failed;
            debugString = "GameCenter Authentication failed";
        }

        private IEnumerator GetDataFromSunDragonNet()
        {
            var debugString = "\nConnecting to sundragon.net : ";
            var request = UnityWebRequest.Get(DataURL);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                debugString += "failed\n" +
                               request.error;
                sundragonNetStatus = LoadStatus.Failed;
                rangking_ui.SetUI(LeaderboardUI.RankUIPage.Failed);
            }
            else
            {
                debugString += "success";

                var data = request.downloadHandler.text;
                var rows = data.Split('\n');

                foreach (var row in rows)
                {
                    var cols = row.Split(',');
                    if (cols[0] == "") continue;
                    PlayerPrefs.SetString(cols[0], cols[1]);
                    debugString += "\n " + cols[0] + " : " + cols[1];
                }

                sundragonNetStatus = LoadStatus.Success;
            }

            DebugText_ui.text += debugString;
            Debug.Log(debugString);
        }

        public enum LoadStatus
        {
            Loading,
            Success,
            Failed
        }
    }
}