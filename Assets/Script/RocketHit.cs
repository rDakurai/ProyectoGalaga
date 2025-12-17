using UnityEngine;

public class RocketHit : MonoBehaviour
{
    private SpecialAttack special;

    private void Awake()
    {
        special = FindObjectOfType<SpecialAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemyHealth = other.GetComponent<EnemyHealth>();
        var rocket = GetComponent<RocketMovement>();

        if (enemyHealth != null && rocket != null)
        {
            enemyHealth.TakeDamage(rocket.Damage);

            if (special != null) special.AddChargeFromHit();
        }

        Destroy(gameObject);
    }
}

