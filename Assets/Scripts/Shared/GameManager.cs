using CloneGame.Launch;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        FirstShot,
        Playing,
        Paused,
        GameOver,
        Win
    }

    [SerializeField] private GameState currentState = GameState.FirstShot;
    [SerializeField] private GameObject levelDesign;
    public GameObject loseScreen;
    public GameObject winScreen;
    [SerializeField] private GameObject[] remainingPigs;
    [SerializeField] private GameObject bonusPopupPrefab;

    private void Awake()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        SetAllRigidbodiesStatic();
    }

    private void OnEnable() => SlingshotController.OnBirdLaunched += HandleBirdLaunched;
    private void OnDisable() => SlingshotController.OnBirdLaunched -= HandleBirdLaunched;

    private void HandleBirdLaunched(Bird bird)
    {
        if (currentState == GameState.FirstShot)
        {
            currentState = GameState.Playing;
            SetAllRigidbodiesDynamic();
        }
    }

    private void Update()
    {
        remainingPigs = GameObject.FindGameObjectsWithTag("Pig");
        switch (currentState)
        {
            case GameState.FirstShot:
                Time.timeScale = 1f;
        
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if (remainingPigs.Length <= 0)
                {
                    CheckForGameOver();
                }
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Win:
                Time.timeScale = 0f;
                break;
        }
    }

    void SetAllRigidbodiesStatic()
    {
        Rigidbody2D[] rbs2d = levelDesign.GetComponentsInChildren<Rigidbody2D>();
        if (rbs2d != null)
        {
            foreach (Rigidbody2D rb2d in rbs2d)
            {
                rb2d.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    void SetAllRigidbodiesDynamic()
    {
        Rigidbody2D[] rbs2d = levelDesign.GetComponentsInChildren<Rigidbody2D>();
        if (rbs2d != null)
        {
            foreach (Rigidbody2D rb2d in rbs2d)
            {
                rb2d.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    public void OnAllBirdsLaunched()
    {
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        Invoke(nameof(DelayedGameOverCheck), 3f);
    }

    private void DelayedGameOverCheck()
    {
        bool hasTargetsRemaining = CheckTargetsRemaining();

        if (hasTargetsRemaining)
        {
            currentState = GameState.GameOver;
            Debug.Log("Game Over! You lost!");
            loseScreen.SetActive(true);
        }
        else
        {
            AwardBonusPointsForRemainingBirds();
            currentState = GameState.Win;
            Debug.Log("You Win!");
            winScreen.SetActive(true);
        }
    }

    private bool CheckTargetsRemaining()
    {
        Debug.Log("Remaining pigs: " + remainingPigs.Length);
        return remainingPigs.Length > 0;
    }

    private void AwardBonusPointsForRemainingBirds()
    {
        SlingshotController slingshot = FindAnyObjectByType<SlingshotController>();
        if (slingshot != null)
        {
            int remainingBirds = slingshot.GetRemainingBirdsCount();
            if (remainingBirds > 0)
            {
                int bonusPoints = remainingBirds * 10000;
                ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
                if (scoreManager != null)
                {
                    scoreManager.UpdateScore(bonusPoints);
                    Debug.Log($"BONUS: {bonusPoints} points for {remainingBirds} remaining birds!");
                }
            }
        }
    }

    private void ShowBonusPopup(int bonusPoints)
    {
        if (bonusPopupPrefab != null)
        {
            GameObject popup = Instantiate(bonusPopupPrefab, winScreen.transform);
            TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"+{bonusPoints} BONUS!";
            }
            Destroy(popup, 2f);
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}