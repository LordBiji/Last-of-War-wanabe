using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;  // Kecepatan maju
    public float health = 20f; // HP musuh
    public int damage = 1;    // Jumlah pawn yang berkurang jika menabrak player

    void Update()
    {
        // Enemy bergerak maju ke arah pemain
        transform.position += Vector3.back * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jika bertabrakan dengan Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ModifyPawnCount(-damage); // Kurangi jumlah pawn
            }
            Destroy(gameObject); // Enemy hancur setelah menabrak player
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
