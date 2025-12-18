using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)]
    private int maxHealth = 2;

    [Header("Animación de Muerte")]
    [SerializeField]
    private GameObject[] deathAnimations; // Array de prefabs de animación

    [SerializeField, Min(0f)]
    private float deathAnimationDuration = 1f; // Duración antes de destruir la animación

    private int _currentHealth;
    private bool _dead;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        _currentHealth = maxHealth;
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (_dead)
        {
            return;
        }

        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        // Reproducir sonido de golpe si aún no muere
        if (_currentHealth > 0)
        {
            EnemyAudioManager.PlayHit();
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (_dead)
        {
            return;
        }

        _dead = true;

        // Reproducir sonido de muerte (boss o normal)
        var bossComponent = GetComponent<Enemy2bComportamiento>();
        if (bossComponent != null)
        {
            // Detener todos los sonidos del boss antes de que suene el de muerte
            EnemyAudioManager.StopBossAttack1();
            EnemyAudioManager.StopBossAttack2();
            EnemyAudioManager.PlayBossDeath();
        }
        else
        {
            EnemyAudioManager.PlayDeath();
        }

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

        // Dropear items antes de destruir
        var dropper = GetComponent<EnemyDropper>();
        if (dropper != null)
        {
            dropper.DropItems();
        }

        // Reproducir animación de muerte intangible
        if (deathAnimations != null && deathAnimations.Length > 0)
        {
            // Seleccionar animación aleatoria del array
            int randomIndex = UnityEngine.Random.Range(0, deathAnimations.Length);
            GameObject selectedAnim = deathAnimations[randomIndex];
            
            if (selectedAnim != null)
            {
                // Instanciar en la posición del enemigo
                GameObject animInstance = Instantiate(selectedAnim, transform.position, transform.rotation);
                
                // Asegurar que no tenga colisiones
                foreach (var col in animInstance.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }
                
                // Destruir después de la duración
                Destroy(animInstance, deathAnimationDuration);
            }
        }

        // Llamar a OnDeath si existe en el enemigo específico
        var kamikaze = GetComponent<EnemyKamikazeComportamiento>();
        if (kamikaze != null)
        {
            // Kamikaze ya tiene integrado el drop, solo destruir
        }

        var shooter = GetComponent<EnemyShooterComportamiento>();
        if (shooter != null)
        {
            // Shooter ya tiene integrado el drop, solo destruir
        }

        var boss = GetComponent<Enemy2bComportamiento>();
        if (boss != null)
        {
            // Boss ya tiene integrado el drop, solo destruir
        }

        Destroy(gameObject);
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
}
