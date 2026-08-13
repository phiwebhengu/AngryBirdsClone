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
    public void Level2()
    {
        SceneManager.LoadSceneAsync(2);
    }
    public void Level3()
    {
        SceneManager.LoadSceneAsync(3);
    }
    public void Level4()
    {
        SceneManager.LoadSceneAsync(4);
    }
    public void Level5()
    {
        SceneManager.LoadSceneAsync(5);
    }

}
