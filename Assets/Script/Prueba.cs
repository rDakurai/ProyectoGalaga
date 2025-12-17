using UnityEngine;

public class Prueba : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageCooldown = 1f;

    private bool canDamage = true;

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
        if (!canDamage) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(damage);
        StartCoroutine(DamageCooldown());
    }

    private System.Collections.IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}
