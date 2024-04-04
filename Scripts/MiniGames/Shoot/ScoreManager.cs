using DG.Tweening;
using TMPro;
using UnityEngine;

namespace DynamicGames.MiniGames.Shoot
{
    /// <summary>
    ///     Manages the score in a shooting mini-game.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private int score;

        private void Start()
        {
            score = 0;
            UpdateUI();
        }

        public void AddScore(int amount)
        {
            score += amount;
            UpdateUI();

            scoreText.gameObject.transform.localScale = Vector3.one;
            scoreText.gameObject.transform.DOPunchScale(Vector3.one * 0.25f, 0.15f);
        }

        public void ResetScore()
        {
            score = 0;
            UpdateUI();
        }

        public int GetScore()
        {
            return score;
        }

        private void UpdateUI()
        {
            scoreText.text = score.ToString();
        }
    }
}