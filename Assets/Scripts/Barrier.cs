using UnityEngine;
using TMPro;

public class Barrier : MonoBehaviour
{
    public int value;
    public float speed = 5f; // Kecepatan barrier bergerak
    public TextMeshProUGUI valueText; // UI untuk menampilkan nilai barrier

    void Start()
    {
        // Set nilai awal secara acak antara -30 hingga 3
        value = Random.Range(-30, 4);
        UpdateValueText();
    }

    void Update()
    {
        // Barrier bergerak maju
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }

    public void ModifyValue(int amount)
    {
        value = Mathf.Min(value + amount, 5); // Batasi nilai maksimal sampai 5
        UpdateValueText();
    }

    void UpdateValueText()
    {
        if (valueText != null)
        {
            valueText.text = value.ToString();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Barrier bertabrakan dengan: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Barrier menyentuh Player!");

            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponentInParent<PlayerController>(); // Ambil dari Parent
                if (player != null)
                {
                    player.ModifyPawnCount(value);
                    Destroy(gameObject);
                }
            }
        }
    }
}
