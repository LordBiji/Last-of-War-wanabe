using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame()
    {
        SceneManager.LoadScene("Level Menu");
    }

    public void exitGame()
    {
        Debug.Log("exit");
        Application.Quit();
    }
}

