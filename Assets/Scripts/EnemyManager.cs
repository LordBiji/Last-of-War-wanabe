using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private List<Boss> activeBosses = new List<Boss>();

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

    // Daftarkan musuh saat di-spawn
    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    // Hapus musuh saat dihancurkan
    public void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    // Daftarkan boss saat di-spawn
    public void RegisterBoss(Boss boss)
    {
        if (!activeBosses.Contains(boss))
        {
            activeBosses.Add(boss);
        }
    }

    // Hapus boss saat dihancurkan
    public void UnregisterBoss(Boss boss)
    {
        if (activeBosses.Contains(boss))
        {
            activeBosses.Remove(boss);
        }
    }

    // Cek apakah semua musuh dan boss sudah dikalahkan
    public bool AreAllEnemiesDefeated()
    {
        return activeEnemies.Count == 0 && activeBosses.Count == 0;
    }
}