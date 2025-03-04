using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject barrierPrefab;
    public GameObject bossPrefab; // Prefab boss
    public Transform[] spawnPoints;
    public float spawnRate = 3f;
    public float enemySpacing = 0.5f;
    public WaveManager waveManager;

    public GameObject bonusHPBarrierPrefab;
    public GameObject weaponBarrierPrefab;

    private HashSet<Transform> usedSpawnPoints = new HashSet<Transform>();
    private bool isBossSpawned = false; // Apakah boss sudah di-spawn di wave ini?

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (waveManager.HasMoreWaves())
        {
            usedSpawnPoints.Clear(); // Reset used spawn points at the start of each wave
            isBossSpawned = false; // Reset status boss
            SpawnWave();
            yield return new WaitForSeconds(spawnRate);
            waveManager.NextWave();
        }
    }

    int GetWeightedSpawnType()
    {
        int[] spawnChances = { 0, 1, 1, 2, 2 }; // 1 = Enemy, 2 = Barrier biasa
        return spawnChances[Random.Range(0, spawnChances.Length)];
    }

    void SpawnEnemies(Transform spawnPoint, WaveData wave)
    {
        int enemyCount = Random.Range(wave.minEnemies, wave.maxEnemies + 1);
        List<GameObject> enemies = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemies.Add(newEnemy);
        }

        ArrangeEnemies(enemies, spawnPoint.position);
        usedSpawnPoints.Add(spawnPoint); // Mark spawn point as used
    }

    void ArrangeEnemies(List<GameObject> enemies, Vector3 spawnPosition)
    {
        int total = enemies.Count;
        int rows = Mathf.CeilToInt(Mathf.Sqrt(total));
        int cols = Mathf.CeilToInt((float)total / rows);
        float centerOffsetX = (cols - 1) * enemySpacing / 2f;
        float centerOffsetZ = (rows - 1) * enemySpacing / 2f;

        for (int i = 0; i < total; i++)
        {
            int row = i / cols;
            int col = i % cols;
            float xPos = (col * enemySpacing) - centerOffsetX;
            float zPos = (row * enemySpacing) - centerOffsetZ;
            enemies[i].transform.position = spawnPosition + new Vector3(xPos, 0, zPos);
        }
    }

    void SpawnBarrier(Transform spawnPoint, WaveData wave)
    {
        GameObject barrierPrefabToSpawn = barrierPrefab; // Default barrier biasa
        int barrierValue = Random.Range(wave.minBarrierValue, wave.maxBarrierValue + 1);

        GameObject barrier = Instantiate(barrierPrefabToSpawn, spawnPoint.position, Quaternion.identity);
        Barrier barrierScript = barrier.GetComponent<Barrier>();

        if (barrierScript != null)
        {
            barrierScript.SetPawnEffect(-barrierValue);
        }

        usedSpawnPoints.Add(spawnPoint); // Mark spawn point as used
    }

    void SpawnWave()
    {
        WaveData currentWave = waveManager.GetCurrentWave();
        if (currentWave == null) return;

        bool isLastRepeat = (waveManager.GetCurrentWaveRepeatIndex() >= waveManager.GetCurrentWaveRepeatMax() - 1);

        // Jika ini adalah pengulangan terakhir dan wave memiliki boss, spawn boss saja
        if (isLastRepeat && currentWave.spawnBoss && !isBossSpawned)
        {
            SpawnBoss(currentWave.bossHP);
            isBossSpawned = true; // Tandai boss sudah di-spawn
            return; // Hentikan spawn obstacle lain
        }

        // Jika bukan pengulangan terakhir atau tidak ada boss, spawn obstacle seperti biasa
        Transform specialSpawnPoint = null;
        if (isLastRepeat && (currentWave.addBonusHP || currentWave.addWeaponBarrier))
        {
            specialSpawnPoint = GetAvailableSpawnPoint();
            if (specialSpawnPoint == null)
            {
                // Jika tidak ada titik spawn tersedia, hapus barrier biasa atau musuh dari satu titik spawn
                specialSpawnPoint = FreeUpSpawnPoint();
            }
        }

        foreach (Transform spawnPoint in spawnPoints)
        {
            // Jika titik spawn ini dipilih untuk special barrier, skip spawn barrier biasa atau musuh di sini
            if (spawnPoint == specialSpawnPoint) continue;

            int spawnType = GetWeightedSpawnType();

            if (spawnType == 1) // Spawn Enemy
            {
                SpawnEnemies(spawnPoint, currentWave);
            }
            else if (spawnType == 2) // Spawn Barrier biasa
            {
                SpawnBarrier(spawnPoint, currentWave);
            }
        }

        // Spawn special barrier hanya di satu titik
        if (specialSpawnPoint != null)
        {
            SpawnSpecialBarriers(currentWave, specialSpawnPoint);
        }
    }

    void SpawnBoss(int bossHP)
    {
        Transform spawnPoint = GetAvailableSpawnPoint();
        if (spawnPoint != null)
        {
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            Boss bossScript = boss.GetComponent<Boss>();
            if (bossScript != null)
            {
                bossScript.SetHP(bossHP); // Atur HP boss
            }
            Debug.Log("Boss muncul dengan HP: " + bossHP);
        }
        else
        {
            Debug.LogWarning("Tidak ada titik spawn tersedia untuk boss!");
        }
    }

    Transform GetAvailableSpawnPoint()
    {
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        availableSpawnPoints.RemoveAll(sp => usedSpawnPoints.Contains(sp));

        if (availableSpawnPoints.Count == 0) return null; // Jika semua titik sudah digunakan, tidak spawn

        return availableSpawnPoints[Random.Range(0, availableSpawnPoints.Count)];
    }

    Transform FreeUpSpawnPoint()
    {
        // Pilih satu titik spawn secara acak dan hapus barrier biasa atau musuh yang ada di sana
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Hapus semua objek (barrier biasa atau musuh) di titik spawn ini
        Collider[] colliders = Physics.OverlapSphere(spawnPoint.position, 0.1f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy") || collider.CompareTag("Barrier"))
            {
                Destroy(collider.gameObject);
            }
        }

        usedSpawnPoints.Remove(spawnPoint); // Bebaskan titik spawn ini
        return spawnPoint;
    }

    void SpawnSpecialBarriers(WaveData wave, Transform specialSpawnPoint)
    {
        if (specialSpawnPoint == null)
        {
            Debug.LogWarning("Tidak ada titik spawn tersedia untuk barrier spesial!");
            return;
        }

        if (wave.addBonusHP)
        {
            SpawnBarrierAt(specialSpawnPoint, bonusHPBarrierPrefab, wave.minBonusHPValue, wave.maxBonusHPValue);
            Debug.Log("Bonus HP Barrier spawned at: " + specialSpawnPoint.position);
        }
        else if (wave.addWeaponBarrier)
        {
            SpawnBarrierAt(specialSpawnPoint, weaponBarrierPrefab, wave.minWeaponBarrierValue, wave.maxWeaponBarrierValue);
            Debug.Log("Weapon Barrier spawned at: " + specialSpawnPoint.position);
        }

        usedSpawnPoints.Add(specialSpawnPoint); // Tandai titik ini sebagai digunakan
    }

    void SpawnBarrierAt(Transform spawnPoint, GameObject prefab, int minValue, int maxValue)
    {
        int barrierValue = Random.Range(minValue, maxValue + 1);
        GameObject barrier = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        Barrier barrierScript = barrier.GetComponent<Barrier>();
        if (barrierScript != null)
        {
            barrierScript.SetPawnEffect(-barrierValue);
        }
    }
}