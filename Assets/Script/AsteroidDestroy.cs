using UnityEngine;
using System.Collections;

public class AsteroidDestroy : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private bool canDamage = true;

    [Header("Animación al morir (sprites)")]
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite[] destroyFrames;
    [SerializeField] private float fps = 12f;
    [SerializeField] private bool disableCollidersOnDestroy = true;

    [Header("Opcional")]
    [SerializeField] private GameObject flameObject;
    [SerializeField] private Behaviour[] disableScriptsOnDeath;

    private bool isDying;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (flameObject == null)
        {
            Transform t = transform.Find("Flame");
            if (t != null) flameObject = t.gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

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

    public void DestroyWithAnimation()
    {
        if (isDying) return;
        StartCoroutine(DestroyRoutine());
    }

    private IEnumerator DestroyRoutine()
    {
        isDying = true;

        if (disableCollidersOnDestroy)
        {
            foreach (var c in GetComponentsInChildren<Collider2D>())
                c.enabled = false;
        }

        if (disableScriptsOnDeath != null)
        {
            foreach (var s in disableScriptsOnDeath)
                if (s != null) s.enabled = false;
        }

        if (flameObject != null) flameObject.SetActive(false);

        if (targetRenderer == null || destroyFrames == null || destroyFrames.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        float wait = 1f / Mathf.Max(1f, fps);

        for (int i = 0; i < destroyFrames.Length; i++)
        {
            if (destroyFrames[i] != null)
                targetRenderer.sprite = destroyFrames[i];

            yield return new WaitForSeconds(wait);
        }

        Destroy(gameObject);
    }
}


