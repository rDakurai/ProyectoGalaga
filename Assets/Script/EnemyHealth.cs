using UnityEngine;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)]
    private int maxHealth = 2;

    [Header("Puntaje")]
    [SerializeField] private int scoreOnDeath = 10;
    [SerializeField] private int bossScoreOnDeath = 100;

    [Header("Boss / Niveles")]
    [SerializeField] private bool isBoss = false;
    [SerializeField] private bool isFinalBoss = false;
    [SerializeField] private string nextLevelScene = "SampleScene2";
    [SerializeField] private float nextLevelDelay = 5f;

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

        // Detectar boss automáticamente si tiene el componente
        if (!isBoss && GetComponent<Enemy2bComportamiento>() != null)
            isBoss = true;
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
            return;

        _dead = true;

        // Sumar puntos al score
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(isBoss ? bossScoreOnDeath : scoreOnDeath);

        // Reproducir sonido de muerte (boss o normal)
        bool bossComponent = GetComponent<Enemy2bComportamiento>() != null;
        if (isBoss || bossComponent)
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
            col.enabled = false;

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
            dropper.DropItems();

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
                    col.enabled = false;
                
                // Destruir después de la duración
                Destroy(animInstance, deathAnimationDuration);
            }
        }

        // Cambio de nivel o victoria (SE HACE EN LevelManager)
        if (isBoss || bossComponent)
        {
            LevelManager lm = FindObjectOfType<LevelManager>();

            if (lm != null)
            {
                lm.ShowMessage("¡Jefe eliminado! Preparando siguiente nivel...", 2f);

                if (isFinalBoss)
                {
                    StartCoroutine(VictoryAfterDelay());
                }
                else
                {
                    lm.nextSceneName = nextLevelScene;
                    lm.nextLevelDelay = nextLevelDelay;
                    lm.LoadNextLevelAfterDelay();
                }
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator VictoryAfterDelay()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        if (GameManager.Instance != null)
            GameManager.Instance.Victory();
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
}
