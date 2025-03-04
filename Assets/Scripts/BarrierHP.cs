using UnityEngine;

public class BarrierHP : Barrier
{
    public int hpIncreaseAmount = 1; // Berapa HP yang ditambahkan ke tiap pawn
    private bool isUnlocked = false; // Apakah barrier sudah terbuka?
    private bool effectApplied = false; // Apakah efek sudah diterapkan?

    void Update()
    {
        transform.Translate(Vector3.back * GlobalSpeed * Time.deltaTime);
    }

    public override void ReceiveShot(float damage)
    {
        if (!isUnlocked)
        {
            base.ReceiveShot(damage); // Panggil logika tembakan dari class Barrier

            if (pawnEffect >= 0)
            {
                isUnlocked = true; // Barrier sudah bisa diambil oleh Player
            }
        }
    }

    void ApplyEffect()
    {
        if (!effectApplied) // Pastikan efek hanya diterapkan sekali
        {
            PlayerController player = PlayerController.Instance;
            if (player != null)
            {
                player.IncreasePawnHP(hpIncreaseAmount); // Berikan efek peningkatan HP
            }
            effectApplied = true; // Tandai efek sudah diterapkan
            Destroy(gameObject); // Hancurkan barrier setelah diambil
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUnlocked && other.CompareTag("Player")) // Jika belum terbuka dan kena Pawn (dengan tag "Player")
        {
            // Hancurkan hanya pawn yang menabrak
            PlayerController player = PlayerController.Instance;
            if (player != null)
            {
                player.DestroyPawnOnCollision(other); // Hancurkan pawn yang menabrak
                player.ArrangePawns(); // Atur ulang formasi
            }
        }
        else if (isUnlocked && other.CompareTag("Player")) // Jika sudah terbuka, Player bisa mengambil buff
        {
            ApplyEffect();
        }
    }
}