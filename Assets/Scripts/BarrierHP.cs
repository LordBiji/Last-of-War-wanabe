using UnityEngine;

public class BarrierHP : Barrier
{
    public int hpIncreaseAmount = 1; // Berapa HP yang ditambahkan ke tiap pawn
    private Transform playerTransform;
    private bool isUnlocked = false; // Apakah barrier sudah terbuka?

    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().transform; // Cari posisi Player
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.Translate(Vector3.back * Barrier.GlobalSpeed * Time.deltaTime);
        }
    }

    public override void ReceiveShot(float damage)
    {
        if (!isUnlocked)
        {
            pawnEffect += 1; // Kurangi nilai negatif menuju nol
            UpdateText();

            if (pawnEffect >= 0)
            {
                isUnlocked = true; //Barrier sudah bisa diambil oleh Player
            }
        }
    }

    void ApplyEffect()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            player.IncreasePawnHP(hpIncreaseAmount);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked && other.CompareTag("Player")) // Jika belum terbuka dan kena Pawn
        {
            Destroy(other.gameObject); //Hancurkan hanya pawn yang menabrak
        }
        else if (isUnlocked && other.CompareTag("Player")) // Jika sudah terbuka, Player bisa mengambil buff
        {
            ApplyEffect();
        }
    }
}
