using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)]
    private int maxHealth = 2;

    private int _currentHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Aquí puedes agregar efectos de muerte, puntuación, etc.
        Destroy(gameObject);
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
}
