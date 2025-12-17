using UnityEngine;

public class HeartHeal : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        if (health.CurrentHealth < health.MaxHealth)
        {
            health.Heal(amount);
            Destroy(gameObject);
        }
    }
}
