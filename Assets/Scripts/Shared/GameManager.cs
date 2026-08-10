using CloneGame.Launch;
using UnityEngine;

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

    void Awake()
    {
       

        Rigidbody2D[] rbs = levelDesign.GetComponentsInChildren<Rigidbody2D>();
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }


    void Update()
    {
        switch (currentState)
        {
            case GameState.FirstShot:

                SetAllRigidbodiesStatic();
                Bird bird = FindAnyObjectByType<Bird>();
                if (bird != null && bird.IsFlying)
                {
                    currentState = GameState.Playing;
                    
                }
                break;
            case GameState.Playing:
              
                    SetAllRigidbodiesDynamic(); //This is where the bird will trigger this state because yeah without this, the buildings go flying.
                   
                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                break;
            case GameState.Win:
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
            currentState = GameState.Win;
            Debug.Log("You Win!");
            winScreen.SetActive(true);
        }
    }

    private bool CheckTargetsRemaining() 
    {
        //For letstatsi if not done by tuesday just do it  if he wants it

        //It must return true if pigs are still alive , false if all pigs are slaugthered
        return true; 
        //winScreen.SetActive(true);
    }
    public void RestartScene ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
}
