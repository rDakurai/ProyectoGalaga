using UnityEngine;

public abstract class ItemPickup : MonoBehaviour
{
    protected abstract void ApplyEffect(GameObject player);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyEffect(other.gameObject);
        Destroy(gameObject);
    }
}
