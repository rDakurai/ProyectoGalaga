using UnityEngine;
using System.Collections;

public class AsteroidDestroy : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private bool canDamage = true;

    // =========================
    // Animación al destruirse
    // =========================
    [Header("Animación al morir (sprites)")]
    [SerializeField] private SpriteRenderer targetRenderer;   // renderer que cambiará sprites
    [SerializeField] private Sprite[] destroyFrames;          // frames de explosión / muerte
    [SerializeField] private float fps = 12f;                 // velocidad animación
    [SerializeField] private bool disableCollidersOnDestroy = true;

    private bool isDying;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (isDying) return;
        if (!canDamage) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(damage);
        StartCoroutine(DamageCooldown());
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }

    // ============================================================
    // LLAMA ESTO en vez de Destroy(gameObject)
    // ============================================================
    public void DestroyWithAnimation()
    {
        if (isDying) return;
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        isDying = true;

        // Opcional: apagar colisiones para que no siga dañando mientras explota
        if (disableCollidersOnDestroy)
        {
            foreach (var c in GetComponents<Collider2D>()) c.enabled = false;
        }

        // Si no hay frames, destruye normal
        if (targetRenderer == null || destroyFrames == null || destroyFrames.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        float wait = 1f / Mathf.Max(1f, fps);

        for (int i = 0; i < destroyFrames.Length; i++)
        {
            targetRenderer.sprite = destroyFrames[i];
            yield return new WaitForSeconds(wait);
        }

        Destroy(gameObject);
    }
}


