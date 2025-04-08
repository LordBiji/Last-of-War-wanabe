using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject winCanvas;
    public GameObject loseCanvas;
    private bool gameEnded = false;

    private WaveManager waveManager;
    private int currentLevel;

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

        // Ambil nomor level saat ini
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("level "))
        {
            currentLevel = int.Parse(sceneName.Split(' ')[1]);
        }
        else
        {
            currentLevel = 1;
        }
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

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextLevel = currentLevel + 1;
        string nextLevelName = "level " + nextLevel;

        // Cek apakah scene tersedia di Build Settings
        if (SceneUtility.GetBuildIndexByScenePath(nextLevelName) != -1)
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.Log("Ini adalah level terakhir!");
            SceneManager.LoadScene("LevelMenu"); // Kembali ke menu jika tidak ada level berikutnya
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