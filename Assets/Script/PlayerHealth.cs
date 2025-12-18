using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida del Jugador")]
    [SerializeField, Min(1)]
    private int maxHealth = 4;

    [Header("Invencibilidad")]
    [SerializeField, Min(0f)]
    private float invincibilityDuration = 1f; // Duración de invencibilidad tras recibir daño

    [Header("Muerte")]
    [SerializeField]
    private GameObject[] deathAnimations; // Array de prefabs de animación de muerte

    [SerializeField, Min(0f)]
    private float deathAnimationDuration = 1f; // Duración de la animación

    [SerializeField, Min(0f)]
    private float delayBeforeEndScene = 4f; // Tiempo total antes de cambiar de escena

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;

    private float _invincibilityTimer;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Update()
    {
        // Decrementar contador de invencibilidad
        if (_invincibilityTimer > 0f)
        {
            _invincibilityTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage = 1)
    {
        // Revisar si está en invencibilidad
        if (_invincibilityTimer > 0f)
        {
            return; // No recibe daño mientras está invencible
        }

        if (damage <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        // Reproducir sonido de golpe (solo si no muere)
        if (CurrentHealth > 0)
        {
            PlayerAudioManager.PlayHit();
            // Activar invencibilidad
            _invincibilityTimer = invincibilityDuration;
        }

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
        // Reproducir sonido de muerte
        PlayerAudioManager.PlayDeath();

        // Spawnear animación de muerte
        if (deathAnimations != null && deathAnimations.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, deathAnimations.Length);
            GameObject selectedAnim = deathAnimations[randomIndex];
            
            if (selectedAnim != null)
            {
                GameObject animInstance = Instantiate(selectedAnim, transform.position, transform.rotation);
                
                // Asegurar que no tenga colisiones
                foreach (var col in animInstance.GetComponentsInChildren<Collider2D>())
                {
                    col.enabled = false;
                }
                
                // Destruir animación después de su duración
                Destroy(animInstance, deathAnimationDuration);
            }
        }

        // Iniciar contador de 4 segundos antes de cambiar de escena
        StartCoroutine(WaitAndLoadEndScene());

        // Ocultar al jugador sin desactivar el GameObject para no detener la corrutina
        HidePlayer();
    }

    private System.Collections.IEnumerator WaitAndLoadEndScene()
    {
        yield return new WaitForSeconds(delayBeforeEndScene);
        SceneManager.LoadScene("End");
    }

    private void HidePlayer()
    {
        // Deshabilitar control y física
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Deshabilitar colisiones
        foreach (var col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        // Ocultar sprites
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
    }
}

