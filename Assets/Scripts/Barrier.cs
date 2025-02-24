using UnityEngine;
using TMPro;

public class Barrier : MonoBehaviour
{
    public int pawnEffect; // Nilai yang diberikan ke Player saat bertabrakan
    public static float GlobalSpeed = 5f; // Kecepatan sentral untuk semua Barrier
    public TextMeshProUGUI valueText;

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
            valueText.text = pawnEffect.ToString();
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
        pawnEffect += 1; // Setiap tembakan, nilainya bertambah
        UpdateText();
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
