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
    [SerializeField]private GameObject[] remainingPigs;
    [SerializeField] private GameObject bonusPopupPrefab;
    void Awake()
    {
       

        Rigidbody2D[] rbs = levelDesign.GetComponentsInChildren<Rigidbody2D>();
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        
       
    }


    void Update()
    {
        remainingPigs = GameObject.FindGameObjectsWithTag("Pig");
        switch (currentState)
        {
            case GameState.FirstShot:
                Time.timeScale= 1f;
                SetAllRigidbodiesStatic();
                SetStateToPlay();
                Bird bird = FindAnyObjectByType<Bird>(); 
                if (bird == null )
        {
            Debug.LogError("No Bird found in the scene.WTF");
        }
                if (bird != null && bird.IsFlying)
                {
                    currentState = GameState.Playing;
                    SetAllRigidbodiesDynamic();
                }
                break;
            case GameState.Playing:
              Time.timeScale = 1f; SetAllRigidbodiesDynamic();
                //This is where the bird will trigger this state because yeah without this, the buildings go flying.
                if (remainingPigs.Length<=0)
                {
                    CheckForGameOver();
                }
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                Time.timeScale = 0f; // Pause the game when the player loses
                break;
            case GameState.Win:
                Time.timeScale = 0f; // Pause the game when the player wins
                break;

        }

    }
    void SetStateToPlay()
    {
        int waitTime = 5;

        for (int i = 0; i <= waitTime; i++)
        {
           
            if (i == waitTime)
            {
                currentState = GameState.Playing;
            }
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
      
        Invoke("DelayedGameOverCheck", 3f);
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
        if (remainingPigs.Length <= 0)
        {
            
            return false;
        }
        return true; 
        //winScreen.SetActive(true);
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
                    Debug.Log($"🎉 BONUS: {bonusPoints} points for {remainingBirds} remaining birds!");
                }
            }
        }
    }
    private void ShowBonusPopup(int bonusPoints) //If i have time 
    {
        if (bonusPopupPrefab != null)
        {
            GameObject popup = Instantiate(bonusPopupPrefab, winScreen.transform);
            TextMeshProUGUI text = popup.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = $"+{bonusPoints} BONUS!";
            }
            Destroy(popup, 2f); // Auto-destroy after 2 seconds
        }
    }
    public void RestartScene ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
