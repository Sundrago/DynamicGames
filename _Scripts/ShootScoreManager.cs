using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShootScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI score_text;
    private int score;

    void Start()
    {
        score = 0;
        UpdateUI();
    }
    
    public void AddScore(int amount) {
        score += amount; 
        UpdateUI();
    }

    public void ResetScore() {
        score = 0;
        UpdateUI();
    }

    public int GetScore() {
        return score;
    }

    private void UpdateUI(){
        score_text.text = score.ToString();
    }
}
