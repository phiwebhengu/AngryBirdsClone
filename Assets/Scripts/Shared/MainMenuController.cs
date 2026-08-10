using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject levelScreen;
    void Start()
    {
        mainMenu.SetActive(true);
        levelScreen.SetActive(false);
    }

   
    public void Play()
    {
        mainMenu.SetActive(false);
        levelScreen.SetActive(true);
    }
    public void Level1()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
