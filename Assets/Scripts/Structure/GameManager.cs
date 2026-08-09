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
    private GameState currentState = GameState.FirstShot;
    [SerializeField] private GameObject levelDesign;
    
    void Awake()
    {
        Rigidbody2D[] rbs = levelDesign.GetComponentsInChildren<Rigidbody2D>();
    }


    void Update()
    {
        switch (currentState)
        {
            case GameState.FirstShot:

                SetAllRigidbodiesStatic();

                if (Input.GetKey(KeyCode.Space)) //Remove the input when you do this because the bird must trigger this state or things wont be like angry birds this is just testing
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
}
