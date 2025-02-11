using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject pawnPrefab;
    public Transform pawnParent;
    public GameObject bulletPrefab;
    public float spacing = 1.5f;
    public float speed = 5f;
    public float minX = -5f;
    public float maxX = 5f;
    public float fireRate = 0.5f;
    private List<GameObject> pawns = new List<GameObject>();
    private float nextFireTime;

    public int maxPawnCount = 20; // Batas maksimal pawn
    private int extraPawnCount = 0; // Jumlah pawn tambahan
    public float baseBulletDamage = 10f; // Damage dasar peluru

    private static PlayerController instance; // Singleton untuk referensi global

    public static PlayerController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AddPawn();
    }

    void Update()
    {
        float moveX = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) moveX = 1f;

        Vector3 newPosition = transform.position + Vector3.right * moveX * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        transform.position = newPosition;

        if (Time.time >= nextFireTime)
        {
            FireBullets();
            nextFireTime = Time.time + fireRate;
        }
    }

    public void ModifyPawnCount(int amount)
    {
        if (amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                if (pawns.Count < maxPawnCount)
                {
                    AddPawn();
                }
                else
                {
                    extraPawnCount++; // Jika lebih dari 20, jadi tambahan damage
                }
            }
        }
        else if (amount < 0)
        {
            for (int i = 0; i < -amount; i++)
            {
                RemovePawn();
            }
        }
        ArrangePawns();
    }

    void AddPawn()
    {
        GameObject newPawn = Instantiate(pawnPrefab, pawnParent);
        pawns.Add(newPawn);
        ArrangePawns();
    }

    void RemovePawn()
    {
        if (pawns.Count > 0)
        {
            GameObject lastPawn = pawns[pawns.Count - 1];
            pawns.Remove(lastPawn);
            Destroy(lastPawn);
        }

        if (pawns.Count < 1)
        {
            GameOver();
        }
    }

    void ArrangePawns()
    {
        int count = pawns.Count;
        float radiusStep = spacing * 0.5f;
        float angleOffset = 137.5f;

        for (int i = 0; i < count; i++)
        {
            float radius = Mathf.Sqrt(i) * radiusStep;
            float angle = i * angleOffset * Mathf.Deg2Rad;

            float xPos = Mathf.Cos(angle) * radius;
            float zPos = Mathf.Sin(angle) * radius;

            pawns[i].transform.localPosition = new Vector3(xPos, 0, zPos);
        }
    }

    void FireBullets()
    {
        float extraDamage = (float)extraPawnCount / 20f;
        float bulletDamage = baseBulletDamage + extraDamage;

        foreach (GameObject pawn in pawns)
        {
            GameObject bullet = Instantiate(bulletPrefab, pawn.transform.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
            }
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Player has no more pawns.");
        // Tambahkan logika seperti UI game over atau kembali ke menu
    }
}