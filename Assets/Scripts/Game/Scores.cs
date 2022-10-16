using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scores : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int _currentScores;

    private void Start()
    {
        _currentScores = 0;
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
    }

    private void AddScores(int score)
    {
        _currentScores += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScores.ToString();
    }
}
