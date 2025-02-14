using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winCanvas;
    public GameObject loseCanvas;
    private bool gameEnded = false;

    private Timer gameTimer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        gameTimer = FindFirstObjectByType<Timer>(); // Ambil script Timer
    }

    void Update()
    {
        if (!gameEnded)
        {
            CheckGameOver(); // Periksa apakah pawn habis sebelum waktu habis
        }
    }

    public void CheckGameOver()
    {
        if (PlayerController.Instance.GetPawnCount() < 1)
        {
            LoseGame();
        }
    }

    public void OnTimeUp()
    {
        if (gameEnded) return;

        if (PlayerController.Instance.GetPawnCount() > 0)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
    }

    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        winCanvas.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        loseCanvas.SetActive(true);
        Time.timeScale = 0f; // Pause game
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("exit");
        Application.Quit();
    }
}
