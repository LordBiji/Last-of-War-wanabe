using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject pawnPrefab;
    public Transform pawnParent;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public float shootRange = 20f;
    public float shootDamage = 10f;
    public LayerMask shootableLayers;
    public GameObject[] bulletPrefabs; // Array prefab peluru berdasarkan DPS
    public float bulletSpeed = 10f; // Kecepatan peluru
    public float bulletLifetime = 2f; // Durasi peluru sebelum dihancurkan
    public float bulletRadius = 0.5f; // Radius deteksi tabrakan peluru

    public GameObject[] pawnModels; // Array prefab model Pawn berdasarkan level HP
    public int pawnHP = 3; // HP awal setiap pawn
    public float pawnDPS = 1.0f; // Default DPS pawn

    private List<GameObject> pawns = new List<GameObject>();
    private float nextFireTime;
    private float moveDirection = 0f;
    private float movementLimit = 5f;

    private Vector2 startTouchPosition, endTouchPosition;
    private bool isSwiping = false;

    private static PlayerController instance;
    public static PlayerController Instance => instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        AddPawn();
    }

    void Update()
    {
        UpdateMovementLimit();
        HandleMovement();

        if (Time.time >= nextFireTime)
        {
            FireBullets();
            nextFireTime = Time.time + fireRate;
        }
    }

    // Update batas gerakan berdasarkan jumlah pawn
    void UpdateMovementLimit()
    {
        int layers = Mathf.CeilToInt(Mathf.Sqrt(pawns.Count));
        movementLimit = 5f - (layers - 1) * 0.5f;
        movementLimit = Mathf.Max(movementLimit, 2f);
    }

    void HandleMovement()
    {
        moveDirection = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKey(KeyCode.LeftArrow)) moveDirection = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveDirection = 1f;
#endif

#if UNITY_ANDROID || UNITY_IOS
        HandleSwipe();
#endif

        MovePawns(moveDirection);
    }

    void HandleSwipe()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                endTouchPosition = touch.position;
                float difference = endTouchPosition.x - startTouchPosition.x;
                moveDirection = Mathf.Clamp(difference / Screen.width * 2f, -1f, 1f);
                startTouchPosition = endTouchPosition;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isSwiping = false;
                moveDirection = 0f;
            }
        }
    }

    void MovePawns(float direction)
    {
        Vector3 movement = Vector3.right * direction * speed * Time.deltaTime;

        // Gerakkan parent (player) dan semua pawn akan mengikuti
        Vector3 newPosition = transform.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, -movementLimit, movementLimit);
        transform.position = newPosition;
    }

    public void ModifyPawnCount(int amount)
    {
        if (amount > 0)
        {
            for (int i = 0; i < amount; i++) AddPawn();
        }
        else if (amount < 0)
        {
            for (int i = 0; i < -amount; i++) RemovePawn();
        }

        ArrangePawns(); // Atur ulang formasi langsung setelah perubahan jumlah pawn
    }

    void AddPawn()
    {
        GameObject newPawn = Instantiate(pawnPrefab, pawnParent);
        pawns.Add(newPawn);
        UpdatePawnModel(newPawn); // Perbarui model Pawn saat ditambahkan
        ArrangePawns(); // Atur ulang formasi langsung setelah penambahan pawn
    }

    void RemovePawn()
    {
        if (pawns.Count > 0)
        {
            GameObject lastPawn = pawns[pawns.Count - 1];
            pawns.Remove(lastPawn); // Hapus dari daftar sebelum dihancurkan
            Destroy(lastPawn);
            ArrangePawns(); // Atur ulang formasi langsung setelah penghancuran pawn
        }

        if (pawns.Count < 1)
        {
            GameManager.Instance.CheckGameOver();
        }
    }

    public void ArrangePawns()
    {
        // Hapus referensi yang null sebelum mengatur ulang formasi
        pawns.RemoveAll(pawn => pawn == null);

        int count = pawns.Count;
        float radiusStep = 1f;
        int layers = Mathf.CeilToInt(Mathf.Sqrt(count));

        int currentIndex = 0;
        for (int layer = 0; layer < layers && currentIndex < count; layer++)
        {
            int pointsInLayer = layer == 0 ? 1 : (layer * 6);
            float angleStep = 360f / Mathf.Max(1, pointsInLayer);

            for (int i = 0; i < pointsInLayer && currentIndex < count; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float xPos = Mathf.Cos(angle) * (layer * radiusStep);
                float zPos = Mathf.Sin(angle) * (layer * radiusStep);

                if (pawns[currentIndex] != null) // Pastikan pawn belum dihancurkan
                {
                    pawns[currentIndex].transform.localPosition = new Vector3(xPos, 0, zPos);
                }
                currentIndex++;
            }
        }
    }

    public void IncreasePawnHP(int amount)
    {
        pawnHP += amount;
        Debug.Log("Pawn HP meningkat: " + amount);

        // Perbarui model Pawn berdasarkan HP
        UpdatePawnModels();
    }

    void UpdatePawnModels()
    {
        foreach (GameObject pawn in pawns)
        {
            if (pawn != null)
            {
                UpdatePawnModel(pawn);
            }
        }
    }

    void UpdatePawnModel(GameObject pawn)
    {
        // Tentukan level HP Pawn
        int hpLevel = Mathf.FloorToInt(pawnHP / 5); // Contoh: Setiap 10 HP, naik level
        hpLevel = Mathf.Clamp(hpLevel, 0, pawnModels.Length - 1); // Pastikan level tidak melebihi jumlah prefab

        // Ganti model Pawn
        GameObject newModel = Instantiate(pawnModels[hpLevel], pawn.transform.position, pawn.transform.rotation, pawn.transform);
        Destroy(pawn.transform.GetChild(0).gameObject); // Hapus model lama
    }

    public void IncreasePawnDPS(float amount)
    {
        pawnDPS += amount;
        Debug.Log("Pawn DPS meningkat: " + amount);
    }

    void FireBullets()
    {
        foreach (GameObject pawn in pawns)
        {
            if (pawn != null) // Pastikan pawn belum dihancurkan
            {
                Vector3 shootOrigin = pawn.transform.position + pawn.transform.forward * 1f; // Titik tembak di depan pawn
                shootOrigin.y += 0.5f; // Menambah offset ke atas (bisa disesuaikan nilainya)
                ShootRaycast(shootOrigin, pawn.transform.forward);
            }
        }
    }

    void ShootRaycast(Vector3 shootOrigin, Vector3 shootDirection)
    {
        RaycastHit hit;

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootRange, shootableLayers))
        {
            // Jika mengenai Barrier
            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.ReceiveShot(1); // Kirim damage sebesar 1
            }

            // Jika mengenai Enemy
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveShot(shootDamage);
            }

            // Jika mengenai Boss
            Boss boss = hit.collider.GetComponentInParent<Boss>();
            if (boss != null)
            {
                boss.ReceiveShot(shootDamage);
            }

            // Tampilkan visual peluru ke titik tumbukan
            ShowBulletTrail(shootOrigin, hit.point);
        }
        else
        {
            // Tampilkan visual peluru ke titik maksimum jarak tembak
            Vector3 targetPoint = shootOrigin + shootDirection * shootRange;
            ShowBulletTrail(shootOrigin, targetPoint);
        }
    }

    void ShowBulletTrail(Vector3 start, Vector3 end)
    {
        // Pilih prefab peluru berdasarkan DPS
        int prefabIndex = Mathf.FloorToInt(pawnDPS / 0.5f) % bulletPrefabs.Length; // Contoh: Setiap 10 DPS, ganti peluru
        if (prefabIndex < 0) prefabIndex = 0; // Pastikan tidak negatif
        GameObject bulletPrefab = bulletPrefabs[prefabIndex];

        // Buat instance peluru
        GameObject bullet = Instantiate(bulletPrefab, start, Quaternion.identity);

        // Atur arah peluru
        bullet.transform.forward = (end - start).normalized;

        // Gerakkan peluru ke target
        StartCoroutine(MoveBullet(bullet, start, end));
    }

    IEnumerator MoveBullet(GameObject bullet, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float duration = distance / bulletSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Periksa tabrakan selama peluru bergerak
            if (CheckBulletCollision(bullet.transform.position))
            {
                Destroy(bullet); // Hancurkan peluru jika terkena objek
                yield break; // Hentikan coroutine
            }

            // Gerakkan peluru
            bullet.transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hancurkan peluru setelah mencapai target
        Destroy(bullet);
    }

    bool CheckBulletCollision(Vector3 position)
    {
        // Gunakan SphereCast untuk mendeteksi tabrakan
        RaycastHit hit;
        if (Physics.SphereCast(position, bulletRadius, Vector3.forward, out hit, 0.1f, shootableLayers))
        {
            // Jika mengenai Barrier
            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.ReceiveShot(shootDamage);
                return true; // Peluru hancur
            }

            // Jika mengenai Enemy
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveShot(shootDamage);
                return true; // Peluru hancur
            }
        }

        return false; // Tidak ada tabrakan
    }

    public int GetPawnCount()
    {
        return pawns.Count;
    }

    public void DestroyPawnOnCollision(Collider other)
    {
        for (int i = 0; i < pawns.Count; i++)
        {
            if (pawns[i] != null && pawns[i].GetComponent<Collider>() == other)
            {
                GameObject pawnToDestroy = pawns[i];
                pawns.RemoveAt(i); // Hapus dari daftar
                Destroy(pawnToDestroy); // Hancurkan pawn
                ArrangePawns(); // Atur ulang formasi setelah penghancuran
                break;
            }
        }

        // Periksa apakah pawn sudah habis
        if (pawns.Count < 1)
        {
            GameManager.Instance.CheckGameOver();
        }
    }
}