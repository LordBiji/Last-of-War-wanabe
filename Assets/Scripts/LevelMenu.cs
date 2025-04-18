using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false; // Nonaktifkan tombol level
        }
        for (int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true; // Aktifkan tombol level yang sudah terbuka
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "level " + levelId;
        SceneManager.LoadScene(levelName);
    }
}