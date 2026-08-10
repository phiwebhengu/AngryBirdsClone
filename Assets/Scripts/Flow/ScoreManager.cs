using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float currentScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    void Awake()
    {
        currentScore = 0f;
    }
    public void UpdateScore (float score)
    {

        currentScore += (int)score;

        scoreText.text = $"Points: {currentScore}";
        Debug.Log($"Score updated by: {score}, Total: {currentScore}");
    }
}
