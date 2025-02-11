using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject barrierPrefab;
    public Transform[] spawnPoints; // 3 titik spawn
    public float spawnRate = 3f; // Waktu antar spawn
    public float enemySpacing = 0.5f; // Jarak antar enemy dalam formasi
    public int minEnemies = 3;
    public int maxEnemies = 10;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnWave), 2f, spawnRate);
    }

    void SpawnWave()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            int spawnType = Random.Range(0, 3); // 0 = Tidak spawn, 1 = Enemy, 2 = Barrier

            if (spawnType == 1)
            {
                SpawnEnemies(spawnPoint);
            }
            else if (spawnType == 2)
            {
                SpawnBarrier(spawnPoint);
            }
        }
    }

    void SpawnEnemies(Transform spawnPoint)
    {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        List<GameObject> enemies = new List<GameObject>();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemies.Add(newEnemy);
        }

        ArrangeEnemies(enemies, spawnPoint.position);
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

    void SpawnBarrier(Transform spawnPoint)
    {
        Instantiate(barrierPrefab, spawnPoint.position, Quaternion.identity);
    }
}
