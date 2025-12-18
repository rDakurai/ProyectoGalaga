using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)]
    private int maxHealth = 2;

    private int _currentHealth;
    private bool _dead;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_dead)
        {
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_dead)
        {
            return;
        }

        _dead = true;

        // Evitar más colisiones mientras reproduce la animación
        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        Destroy(gameObject);
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
}
