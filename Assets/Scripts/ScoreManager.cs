using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float currentScore;
    
    void Awake()
    {
        currentScore = 0f;
    }
    public void UpdateScore (float score)
    {
        // Implement your score updating logic here
        currentScore += score;
        Debug.Log($"Score updated by: {score}, Total: {currentScore}");
    }
}
