using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveData
{
    [Header("Enemy Settings")]
    public string waveName; // Nama wave (untuk debugging)
    public int minEnemies;
    public int maxEnemies;

    [Space(10)] // Jarak 10 pixel
    [Header("Barrier Settings")]
    public int minBarrierValue;
    public int maxBarrierValue;
    public int repeatCount = 1; // Berapa kali wave ini diulang sebelum ke wave berikutnya

    [Space(10)] // Jarak 10 pixel
    [Header("Bonus HP Barrier")]
    public bool addBonusHP;
    public int minBonusHPValue;
    public int maxBonusHPValue;

    [Space(10)] // Jarak 10 pixel
    [Header("Weapon Barrier")]
    public bool addWeaponBarrier;
    public int minWeaponBarrierValue;
    public int maxWeaponBarrierValue;

    [Space(10)] // Jarak 10 pixel
    [Header("Boss Settings")]
    public bool spawnBoss; // Apakah wave ini memiliki boss?
    public int bossHP; // HP boss
}

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves;
    public int currentWaveIndex = 0;
    private int currentWaveRepeat = 0;

    // Getter untuk mendapatkan wave saat ini
    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    // Setter untuk mengatur wave ke tertentu
    public void SetCurrentWaveIndex(int index)
    {
        if (index >= 0 && index < waves.Count)
        {
            currentWaveIndex = index;
            currentWaveRepeat = 0; // Reset ulang repeat count agar tidak bermasalah
        }
        else
        {
            Debug.LogWarning("Wave index di luar batas! Pastikan index dalam range yang benar.");
        }
    }

    public WaveData GetCurrentWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            return waves[currentWaveIndex];
        }
        return null;
    }

    public int GetCurrentWaveRepeatIndex()
    {
        return currentWaveRepeat;
    }

    public int GetCurrentWaveRepeatMax()
    {
        if (waves != null && currentWaveIndex < waves.Count)
        {
            return waves[currentWaveIndex].repeatCount;
        }
        return 0;
    }

    public void NextWave()
    {
        if (currentWaveIndex < waves.Count)
        {
            currentWaveRepeat++;

            // Jika masih dalam repeat count, tetap gunakan wave yang sama
            if (currentWaveRepeat < waves[currentWaveIndex].repeatCount)
            {
                return;
            }

            // Jika sudah mencapai batas repeat count, pindah ke wave berikutnya
            currentWaveIndex++;
            currentWaveRepeat = 0; // Reset ulang repeat count untuk wave berikutnya
        }
    }

    public bool HasMoreWaves()
    {
        return currentWaveIndex < waves.Count;
    }
}