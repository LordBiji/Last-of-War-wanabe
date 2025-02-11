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
        // Jika terkena bullet
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.bulletValue); // Ambil damage dari bullet
            }
            Destroy(other.gameObject); // Hancurkan bullet
        }
        // Jika bertabrakan dengan Player
        else if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ModifyPawnCount(-damage); // Kurangi jumlah pawn
            }
            Destroy(gameObject); // Enemy hancur setelah menabrak player
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject); // Musuh hancur jika HP habis
        }
    }
}
