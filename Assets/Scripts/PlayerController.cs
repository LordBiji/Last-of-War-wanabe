using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject pawnPrefab;
    public Transform pawnParent;
    public float speed = 5f;
    public float fireRate = 0.5f;
    public Transform shootPoint;
    public float shootRange = 20f;
    public float shootDamage = 10f;
    public LayerMask shootableLayers;
    public LineRenderer bulletTrail;
    public float bulletTrailDuration = 0.05f;

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

        foreach (GameObject pawn in pawns)
        {
            Rigidbody rb = pawn.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 newPosition = rb.position + movement;
                newPosition.x = Mathf.Clamp(newPosition.x, -movementLimit, movementLimit);
                rb.MovePosition(newPosition);
            }
        }
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

        ArrangePawns();
        GameManager.Instance.CheckGameOver();
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
            GameManager.Instance.CheckGameOver();
        }
    }

    void ArrangePawns()
    {
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

                pawns[currentIndex].transform.localPosition = new Vector3(xPos, 0, zPos);
                currentIndex++;
            }
        }
    }

    public void IncreasePawnHP(int amount)
    {
        pawnHP += amount;
        Debug.Log("Pawn HP meningkat: " + amount);
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
            ShootRaycast(pawn.transform.position);
        }
    }

    void ShootRaycast(Vector3 shootOrigin)
    {
        RaycastHit hit;
        Vector3 shootDirection = transform.forward;

        if (Physics.Raycast(shootOrigin, shootDirection, out hit, shootRange, shootableLayers))
        {
            // Jika mengenai Barrier
            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.ReceiveShot(shootDamage);
            }

            // Jika mengenai Enemy
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveShot(shootDamage);
            }

            ShowBulletTrail(shootOrigin, hit.point);
        }
        else
        {
            Vector3 targetPoint = shootOrigin + shootDirection * shootRange;
            ShowBulletTrail(shootOrigin, targetPoint);
        }
    }


    void ShowBulletTrail(Vector3 start, Vector3 end)
    {
        if (bulletTrail != null)
        {
            bulletTrail.SetPosition(0, start);
            bulletTrail.SetPosition(1, end);
            bulletTrail.enabled = true;
            Invoke(nameof(HideBulletTrail), bulletTrailDuration);
        }
    }

    void HideBulletTrail()
    {
        bulletTrail.enabled = false;
    }

    public int GetPawnCount()
    {
        return pawns.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            ModifyPawnCount(-1);
        }
        else if (other.CompareTag("Enemy"))
        {
            ModifyPawnCount(-1);
            Destroy(other.gameObject);
        }
    }
}
