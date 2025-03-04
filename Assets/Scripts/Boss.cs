using UnityEngine;

public class Boss : MonoBehaviour
{
    public float moveSpeed = 2f; // Kecepatan gerak boss (menggunakan global speed)
    private int currentHP; // HP boss (diatur oleh WaveManager)
    private bool isDefeated = false; // Apakah boss sudah dikalahkan?
    private bool hasAttacked = false; // Apakah boss sudah menyerang?

    void Start()
    {
        moveSpeed = Barrier.GlobalSpeed; // Gunakan global speed

        // Daftarkan diri ke EnemyManager saat di-spawn
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterBoss(this);
        }
        else
        {
            Debug.LogWarning("EnemyManager belum diinisialisasi!");
        }
    }

    void OnDestroy()
    {
        // Hapus diri dari EnemyManager saat dihancurkan
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterBoss(this);
        }
    }

    void Update()
    {
        if (isDefeated) return; // Jika boss sudah kalah, hentikan update

        // Bergerak maju
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        // Cek jarak dengan player
        if (!hasAttacked && Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 5f)
        {
            AttackPlayer();
        }
    }

    // Dipanggil saat boss terkena tembakan
    public void ReceiveShot(float damage)
    {
        if (isDefeated) return; // Jika boss sudah kalah, abaikan

        currentHP -= (int)damage;
        Debug.Log($"Boss terkena tembakan! HP tersisa: {currentHP}");

        if (currentHP <= 0)
        {
            DefeatBoss();
        }
    }

    // Dipanggil saat boss dikalahkan
    private void DefeatBoss()
    {
        isDefeated = true;
        Debug.Log("Boss dikalahkan!");
        Destroy(gameObject); // Hancurkan boss
    }

    // Dipanggil saat boss menyerang player
    private void AttackPlayer()
    {
        hasAttacked = true; // Tandai boss sudah menyerang
        moveSpeed = 0f; // Berhenti bergerak

        PlayerController player = PlayerController.Instance;
        if (player != null)
        {
            // Hancurkan semua pawn
            player.ModifyPawnCount(-player.GetPawnCount());
            Debug.Log("Boss menyerang dan menghancurkan semua pawn!");

            // Cek apakah player kalah
            if (player.GetPawnCount() < 1)
            {
                GameManager.Instance.CheckGameOver();
            }
        }
    }

    // Method untuk mengatur HP boss (dipanggil oleh WaveManager)
    public void SetHP(int hp)
    {
        currentHP = hp;
        Debug.Log($"Boss HP diatur menjadi: {currentHP}");
    }
}