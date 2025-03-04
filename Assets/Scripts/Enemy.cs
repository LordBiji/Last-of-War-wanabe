using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;  // Kecepatan maju
    public float health = 20f; // HP musuh
    public int damage = 1;    // Jumlah pawn yang berkurang jika menabrak player


    void Start()
    {
        // Daftarkan diri ke EnemyManager saat di-spawn
        EnemyManager.Instance.RegisterEnemy(this);
    }

    void OnDestroy()
    {
        // Hapus diri dari EnemyManager saat dihancurkan
        EnemyManager.Instance.UnregisterEnemy(this);
    }
    void Update()
    {
        // Enemy bergerak maju ke arah pemain
        transform.position += Vector3.back * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jika bertabrakan dengan Pawn (dengan tag "Player")
        if (other.CompareTag("Player"))
        {
            PlayerController player = PlayerController.Instance;
            if (player != null)
            {
                // Hancurkan Pawn yang bertabrakan
                player.DestroyPawnOnCollision(other);
            }
            Destroy(gameObject); // Enemy hancur setelah menabrak Pawn
        }
    }

    // Metode ini dipanggil saat terkena Raycast dari Player
    public void ReceiveShot(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Enemy hancur jika HP habis
        }
    }
}