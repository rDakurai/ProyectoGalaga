using UnityEngine;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField, Min(1)] private int maxHealth = 2;

    [Header("Score")]
    [SerializeField] private int scoreOnDeath = 10;

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
    private GameObject[] deathAnimations;

    [SerializeField, Min(0f)]
    private float deathAnimationDuration = 1f;

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
        if (_dead) return;

        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (_currentHealth > 0)
            EnemyAudioManager.PlayHit();

        if (_currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        if (_dead) return;
        _dead = true;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreOnDeath);

        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Drop items
        var dropper = GetComponent<EnemyDropper>();
        if (dropper != null)
            dropper.DropItems();

        // Animación de muerte
        if (deathAnimations != null && deathAnimations.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, deathAnimations.Length);
            GameObject anim = deathAnimations[index];

            if (anim != null)
            {
                GameObject instance = Instantiate(anim, transform.position, transform.rotation);

                foreach (var col in instance.GetComponentsInChildren<Collider2D>())
                    col.enabled = false;

                Destroy(instance, deathAnimationDuration);
            }
        }

        // ✅ CAMBIO DE NIVEL O VICTORIA (SE HACE EN LevelManager)
        if (isBoss || bossComponent)
        {
            LevelManager lm = FindObjectOfType<LevelManager>();

            if (lm != null)
            {
                lm.ShowMessage("¡Jefe eliminado! Preparando siguiente nivel...", 2f);

                if (isFinalBoss)
                {
                    lm.StartCoroutine(VictoryAfterDelay());
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

    System.Collections.IEnumerator VictoryAfterDelay()
    {
        yield return new WaitForSeconds(nextLevelDelay);
        if (GameManager.Instance != null)
            GameManager.Instance.Victory();
    }

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
}

