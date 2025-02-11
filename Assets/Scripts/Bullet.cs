using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f; // Default damage
    public int bulletValue = 1;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            Barrier barrier = other.GetComponent<Barrier>();
            if (barrier != null)
            {
                barrier.ModifyValue(bulletValue);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Pastikan ada fungsi TakeDamage di Enemy
            }
            Destroy(gameObject);
        }
    }
}
