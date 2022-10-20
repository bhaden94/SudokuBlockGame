using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class BestScoreData
{
    public int Score = 0;
}

public class Scores : MonoBehaviour
{
    public SquareTextureData squareTextureData;
    public TextMeshProUGUI scoreText;

    private bool _newBestScore;
    private BestScoreData _bestScore = new BestScoreData();
    private int _currentScores;

    private string _bestScoreKey = "bsdat";

    private void Awake()
    {
        if (BinaryDataStream.Exists(_bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        _bestScore = BinaryDataStream.Read<BestScoreData>(_bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.UpdateBestScoreBar(_currentScores, _bestScore.Score);
    }

    private void Start()
    {
        _currentScores = 0;
        _newBestScore = false;
        squareTextureData.SetStartColor();
        UpdateScoreText();
    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScores;
    }

    private void AddScores(int score)
    {
        _currentScores += score;
        if (_currentScores > _bestScore.Score)
        {
            _newBestScore = true;
            _bestScore.Score = _currentScores;
            SaveBestScores(true);
        }

        UpdateSquareColor();
        GameEvents.UpdateBestScoreBar(_currentScores, _bestScore.Score);
        UpdateScoreText();
    }

    private void UpdateSquareColor()
    {
        if (_currentScores >= squareTextureData.thresholdVal)
        {
            squareTextureData.UpdateColor(_currentScores);
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScores.ToString();
    }

    public void SaveBestScores(bool newBestScore)
    {
        // update this to use cloud saves
        BinaryDataStream.Save<BestScoreData>(_bestScore, _bestScoreKey);
    }
}
