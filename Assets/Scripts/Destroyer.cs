using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Barrier") || other.CompareTag("Boss"))
        {
            Destroy(other.gameObject);
        }
    }
}
