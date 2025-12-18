using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    [SerializeField] private int amount = 1;
    [SerializeField] private bool healToMax = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.AddMaxHealth(amount, healToMax);
        PickupManager.Instance.RegisterHeartContainerPickup(amount);
        Destroy(gameObject);
    }
}

