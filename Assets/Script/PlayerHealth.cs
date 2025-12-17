using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del Jugador")]
    [SerializeField, Min(1)]
    private int maxHealth = 4;   // 👈 4 corazones base

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    // La UI se suscribe aquí para actualizar corazones
    public event Action<int, int> OnHealthChanged; // current, max

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void TakeDamage(int damage = 1)
    {
        if (damage <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
            OnDeath();
    }

    public void Heal(int amount = 1)
    {
        if (amount <= 0) return;

        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void AddMaxHealth(int amount = 1, bool healToMax = true)
    {
        if (amount <= 0) return;

        maxHealth += amount;
        if (healToMax) CurrentHealth = maxHealth;

        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }
}

