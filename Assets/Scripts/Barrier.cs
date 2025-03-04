using UnityEngine;
using TMPro;

public class Barrier : MonoBehaviour
{
    public static float GlobalSpeed = 5f; // Kecepatan sentral untuk semua Barrier
    public TextMeshProUGUI valueText;

    protected int pawnEffect; // Nilai yang diberikan ke Player saat bertabrakan (bisa diakses oleh class turunan)
    protected int shotsRequired = 5; // Jumlah tembakan yang dibutuhkan untuk menambah +1 value
    protected int currentShots = 0; // Jumlah tembakan saat ini

    void Start()
    {
        UpdateText();
    }

    void Update()
    {
        transform.Translate(Vector3.back * GlobalSpeed * Time.deltaTime);
    }

    public void UpdateText()
    {
        if (valueText != null)
        {
            valueText.text = pawnEffect.ToString(); // Tampilkan nilai pawnEffect
            valueText.color = (pawnEffect > 0) ? Color.green : Color.red;
        }
    }

    public void SetPawnEffect(int value)
    {
        pawnEffect = value;
        UpdateText();
    }

    public virtual void ReceiveShot(float damage)
    {
        currentShots++; // Tambah jumlah tembakan
        Debug.Log($"Barrier terkena tembakan! Tembakan ke-{currentShots}");

        if (currentShots >= shotsRequired)
        {
            // Jika tembakan mencapai jumlah yang dibutuhkan, tambahkan +1 ke nilai barrier
            pawnEffect += 1;
            currentShots = 0; // Reset jumlah tembakan
            Debug.Log($"Barrier value meningkat menjadi: {pawnEffect}");
            UpdateText();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.ModifyPawnCount(pawnEffect);
            }
            Destroy(gameObject);
        }
    }
}