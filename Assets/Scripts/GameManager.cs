using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winCanvas;
    public GameObject loseCanvas;
    private bool gameEnded = false;

    private WaveManager waveManager;

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
        waveManager = FindFirstObjectByType<WaveManager>(); // Ambil script WaveManager
    }

    void Update()
    {
        if (!gameEnded)
        {
            CheckGameOver(); // Periksa apakah pawn habis
            CheckWinCondition(); // Periksa apakah pemain memenuhi win condition
        }
    }

    public void CheckGameOver()
    {
        if (PlayerController.Instance.GetPawnCount() < 1)
        {
            LoseGame();
        }
    }

    public void CheckWinCondition()
    {
        // Win condition: Semua wave selesai DAN tidak ada musuh atau boss yang tersisa
        if (waveManager.HasMoreWaves() == false && EnemyManager.Instance.AreAllEnemiesDefeated())
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        winCanvas.SetActive(true);
        Time.timeScale = 0f; // Pause game

        // Simpan progress level yang telah diselesaikan
        int currentLevel = int.Parse(SceneManager.GetActiveScene().name.Split(' ')[1]);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            PlayerPrefs.Save();
        }
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