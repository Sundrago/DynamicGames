using System;
using DynamicGames.Gachapon;
using DynamicGames.MiniGames;
using DynamicGames.Pet;
using DynamicGames.UI;
using DynamicGames.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DynamicGames.System
{
    /// <summary>
    ///     Controls the debug features of the game.
    /// </summary>
    public class DebugController : MonoBehaviour
    {
        [Header("Managers and Controllers")] 
        [SerializeField] private TicketsController ticketsController;
        [SerializeField] private SfxController sfxController;
        [SerializeField] private LeaderboardManger leaderboardManger;
        [SerializeField] private LeaderboardUI leaderboardUI;
        [SerializeField] private LocalizationAndFontManager localizationAndFontManager;
        [SerializeField] private TMP_Dropdown localeSelector;

        [Header("UI Components")] 
        [SerializeField] private GameObject islandSizeRemoteWindow;
        [SerializeField] private GameObject debugFrontWindow;

        private int hiddenBtnClickCount;

        private void Start()
        {
            gameObject.SetActive(false);
            islandSizeRemoteWindow.SetActive(false);
        }

        public void OnHiddenButtonClick()
        {
            ++hiddenBtnClickCount;
            if (hiddenBtnClickCount > 5)
            {
                PlayerData.SetInt(DataKey.debugMode, 1);
                EnableDebugFeatures(true);
            }
        }

        private void EnableDebugFeatures(bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
            debugFrontWindow.SetActive(isEnabled);
        }

        public void ShowRankingUI()
        {
            StartCoroutine(leaderboardUI.ShowRankingUI(0));
        }

        public void ShowRandomRanking()
        {
            leaderboardManger.debug_randomRank = true;
            StartCoroutine(leaderboardManger.GetDataFromServers());
        }

        public void Close()
        {
            debugFrontWindow.SetActive(false);
            gameObject.SetActive(false);
        }

        public void DisplayScore()
        {
            ticketsController.TicketAnim(0);
            GameScoreManager.Instance.ShowScore(Random.Range(0, 300), GameType.shoot);
        }

        public void HideScore()
        {
            GameScoreManager.Instance.HideScore();
            sfxController.PlayBGM(-1);
        }

        public void GetTickets(int amount)
        {
            ticketsController.TicketAnim(amount);
        }

        public void AddTickets()
        {
            MoneyManager.Instance.AddTicket(MoneyManager.RewardType.Ticket, 50);
        }

        public void RemoveData()
        {
            PopupTextManager.Instance.ShowYesNoPopup("Resest all player data?", RemoveDataConfirmed);
        }

        private void RemoveDataConfirmed()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            PetManager.Instance.RemoveData();
            Application.Quit();
        }

        public void ShowIslandSizeRemote()
        {
            islandSizeRemoteWindow.SetActive(true);
        }

        public void UnlockAllPets()
        {
            foreach (PetType type in Enum.GetValues(typeof(PetType)))
                if (PetManager.Instance.GetPetCount(type) == 0)
                    PetManager.Instance.AddPetCountByType(type);
        }

        public void ChangeLanguage()
        {
            localizationAndFontManager.FontSelected(localeSelector.value);
        }

        public void AdNetworks()
        {
            IronSource.Agent.launchTestSuite();
        }
    }
}