using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del Jugador")]
    [SerializeField, Min(1)]
    private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        // TODO: actualizar UI o efectos de daño
        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        // Manejo simple: desactivar jugador; puedes reiniciar escena o mostrar game over
        gameObject.SetActive(false);
    }
}
